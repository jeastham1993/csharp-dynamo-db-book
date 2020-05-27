using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure.Extensions;

using Microsoft.Extensions.Logging;

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
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> GetUserAsync(
			string userName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> CreateOrganizationAsync(
			Organization org)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<User>> GetUsersForOrganizationAsync(
			string organizationName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Organization>> GetOrganizationsForUserAsync(
			string userName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> UpdatePaymentPlanForOrganization(
			string organizationName,
			PaymentPlan plan)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<User> UpdatePaymentPlanForUser(
			string userName,
			PaymentPlan plan)
		{
			throw new NotImplementedException();
		}
	}
}
