using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure.Extensions;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;
using DynamoDbBook.GitHubMigration.Core.Infrastructure.Extensions;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure
{
    public class UserRepository : IUserRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<UserRepository> _logger;

		public UserRepository(
			AmazonDynamoDBClient client,
			ILogger<UserRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}
		/// <inheritdoc />
		public async Task<User> CreateUserAsync(
			User user)
		{
			var putItemRequest = new PutItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Item = user.AsItem(),
				ConditionExpression = "attribute_not_exists(PK)"
			};

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return user;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Gist> CreateGist(
			Gist gist)
		{
			var putItemRequest = new PutItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Item = gist.AsItem(),
				ConditionExpression = "attribute_not_exists(PK)"
			};

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return gist;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<User> GetUserAsync(
			string userName)
		{
			var user = new User()
			{
				Username = userName
			};

			var getItemResult = await this._client.GetItemAsync(
				                    new GetItemRequest()
				                    {
					                    TableName = DynamoDbConstants.TableName,
					                    Key = user.AsKeys()
				                    });

			return DynamoHelper.CreateFromItem<User>(getItemResult.Item);
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Gist>> GetGistsAsync(
			string userName)
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#pk", "PK");
			attributeNames.Add("#sk", "SK");

			var user = new User()
			{
				Username = userName
			};

			var attributeValues = user.AsKeys();

			var queryRequest = new QueryRequest()
			{
				TableName = DynamoDbConstants.TableName,
				KeyConditionExpression = "#pk = :pk AND #sk <= :sk",
				ExpressionAttributeNames = attributeNames,
				ExpressionAttributeValues = attributeValues,
				ScanIndexForward = false,
				Limit = 11
			};

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<Gist>();

			foreach (var item in queryResult.Items)
			{
				if (item["Type"].S == "Gist")
				{
					response.Add(DynamoHelper.CreateFromItem<Gist>(item));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<Organization> CreateOrganizationAsync(
			Organization org)
		{
			var user = new User()
			{
				Username = org.OwnerName
			};

			var membership = new Membership()
			{
				MemberSince = DateTime.Now,
				OrganizationName = org.Name,
				Role = "Owner",
				Username = org.OwnerName
			};

			var transactItems = new TransactWriteItemsRequest()
			{
				TransactItems = new List<TransactWriteItem>(3)
				{
					new TransactWriteItem()
					{
						Put = new Put()
						{
							Item = org.AsItem(),
							TableName = DynamoDbConstants.TableName,
							ConditionExpression = "attribute_not_exists(PK)"
						}
					},
					new TransactWriteItem()
					{
						Put = new Put()
						{
							Item = membership.AsItem(),
							TableName = DynamoDbConstants.TableName,
							ConditionExpression = "attribute_not_exists(PK)"
						}
					},
					new TransactWriteItem()
					{
						Update = new Update()
						{
							Key = user.AsKeys(),
							TableName = DynamoDbConstants.TableName,
							UpdateExpression = "SET #data.#organizations.#org = :role",
							ExpressionAttributeNames = new Dictionary<string, string>(3)
							{
								{ "#data", "Data" },
								{ "#organizations", "Organizations" },
								{ "#org", org.Name.ToLower() }
							},
							ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
							{
								{ ":role", new AttributeValue(membership.Role) }
							}
						}
					}
				}
			};

			try
			{
				var result = await this._client.TransactWriteItemsAsync(transactItems).ConfigureAwait(false);

				return org;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task AddUserToOrganizationAsync(
			Membership membership)
		{
			var user = new User() { Username = membership.Username };
			var org = new Organization() { Name = membership.OrganizationName };

			var transactWrite = new TransactWriteItemsRequest()
									{
										TransactItems = new List<TransactWriteItem>(3)
															{
																new TransactWriteItem()
																	{
																		Put = new Put()
																				  {
																					  TableName = DynamoDbConstants.TableName,
																					  Item = membership.AsItem(),
																					  ConditionExpression = "attribute_not_exists(PK)"
																				  },
																	},
																new TransactWriteItem()
																	{
																		Update = new Update()
																					 {
																						 TableName = DynamoDbConstants.TableName,
																						 Key = org.AsKeys(),
																						 ConditionExpression = "attribute_exists(PK)",
																						 UpdateExpression = "SET #data.#members = #data.#members + :inc",
																						 ExpressionAttributeNames = new Dictionary<string, string>(2)
																														{
																															{ "#data", "Data" },
																															{ "#members", "Members" }
																														},
																						 ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																														 {
																															 { ":inc", new AttributeValue() { N = "1"} }
																														 }
																					 }
																	},
																new TransactWriteItem()
																	{
																		Update = new Update()
																					 {
																						 TableName = DynamoDbConstants.TableName,
																						 Key = user.AsKeys(),
																						 ConditionExpression = "attribute_exists(PK)",
																						 UpdateExpression = "SET #data.#organizations.#org = :role",
																						 ExpressionAttributeNames = new Dictionary<string, string>(2)
																														{
																															{ "#data", "Data" },
																															{ "#organizations", "Organizations" },
																															{ "#org", membership.OrganizationName.ToLower() }
																														},
																						 ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																														 {
																															 { ":role", new AttributeValue(membership.Role) }
																														 }
																					 }
																	}
															}
									};

			try
			{
				var response = await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(
					ex,
					"Conditional check failed");
			}
		}

		/// <inheritdoc />
		public async Task<Organization> GetOrganizationAsync(
			string organizationName)
		{
			var org = new Organization()
			{
				Name = organizationName
			};

			var getItemResult = await this._client.GetItemAsync(
				                    new GetItemRequest()
				                    {
					                    TableName = DynamoDbConstants.TableName,
					                    Key = org.AsKeys()
				                    });

			return DynamoHelper.CreateFromItem<Organization>(getItemResult.Item);
		}

		/// <inheritdoc />
		public async Task<List<User>> GetUsersForOrganizationAsync(
			string organizationName)
		{
			var org = new Organization()
			{
				Name = organizationName
			};

			var queryRequest = new QueryRequest()
			{
				TableName = DynamoDbConstants.TableName,
				KeyConditionExpression = "#pk = :pk",
				ExpressionAttributeNames = new Dictionary<string, string>(1)
				{
					{ "#pk", "PK" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
				{
					{ ":pk", new AttributeValue(org.GetPk()) }
				},
				Limit = 26
			};

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			if (queryResult.Count == 0)
			{
				throw new ArgumentException($"Organization {organizationName} not found");
			}

			var response = new List<User>(queryResult.Count - 1); // -1 to remove the actual organization object.

			foreach (var item in queryResult.Items)
			{
				if (item["Type"].S == "Issue")
				{
					response.Add(DynamoHelper.CreateFromItem<User>(item));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<Organization> UpdatePaymentPlanForOrganization(
			string organizationName,
			PaymentPlan plan)
		{
			var org = new Organization()
			{
				Name = organizationName
			};

			var updateItem = new UpdateItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = org.AsKeys(),
				ConditionExpression = "attribute_exists(PK) AND #type = :type",
				UpdateExpression = "SET #data.#paymentPlan = :paymentPlan",
				ExpressionAttributeNames = new Dictionary<string, string>(3)
				{
					{ "#data", "Data" },
					{ "#paymentPlan", "PaymentPlan" },
					{ "#type", "Type" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(2)
				{
					{ ":type", new AttributeValue("Organization") },
					{ ":paymentPlan", new AttributeValue() { M = Document.FromJson(JsonConvert.SerializeObject(plan)).ToAttributeMap()} }
				},
				ReturnValues = ReturnValue.ALL_NEW
			};

			var updateItemResponse = await this._client.UpdateItemAsync(updateItem).ConfigureAwait(false);

			return DynamoHelper.CreateFromItem<Organization>(updateItemResponse.Attributes);
		}

		/// <inheritdoc />
		public async Task<User> UpdatePaymentPlanForUser(
			string userName,
			PaymentPlan plan)
		{
			var user = new User()
			{
				Username = userName
			};

			var updateItem = new UpdateItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = user.AsKeys(),
				ConditionExpression = "attribute_exists(PK) AND #type = :type",
				UpdateExpression = "SET #data.#paymentPlan = :paymentPlan",
				ExpressionAttributeNames = new Dictionary<string, string>(3)
				{
					{ "#data", "Data" },
					{ "#paymentPlan", "PaymentPlan" },
					{ "#type", "Type" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(2)
				{
					{ ":type", new AttributeValue("User") },
					{ ":paymentPlan", new AttributeValue() { M = Document.FromJson(JsonConvert.SerializeObject(plan)).ToAttributeMap()} }
				},
				ReturnValues = ReturnValue.ALL_NEW
			};

			var updateItemResponse = await this._client.UpdateItemAsync(updateItem).ConfigureAwait(false);

			return DynamoHelper.CreateFromItem<User>(updateItemResponse.Attributes);
		}
	}
}
