using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure.Extensions;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure
{
    public class RepoRepository : IRepoRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<RepoRepository> _logger;

		public RepoRepository(
			AmazonDynamoDBClient client,
			ILogger<RepoRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Repository> GetAsync(
			string ownerName,
			string repoName)
		{
			var getItemRequest = new GetItemRequest(
				DynamoDbConstants.TableName,
				new Repository() { OwnerName = ownerName, RepositoryName = repoName }.AsKeys());

			var getItem = await this._client.GetItemAsync(getItemRequest).ConfigureAwait(false);

			return DynamoHelper.CreateFromItem<Repository>(getItem.Item);
		}

		/// <inheritdoc />
		public async Task<Repository> CreateAsync(
			Repository repo)
		{
			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = repo.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return repo;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<List<Issue>> GetIssuesAsync(
			string ownerName,
			string repoName,
			string status = "Open")
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#pk", "PK");
			attributeNames.Add("#sk", "SK");
			attributeNames.Add("#data", "data");
			attributeNames.Add("#status", "status");

			var repo = new Repository() { OwnerName = ownerName, RepositoryName = repoName };

			var attributeValues = new Dictionary<string, AttributeValue>(3);
			attributeValues.Add(
				":pk",
				new AttributeValue($"REPO#{ownerName.ToLower()}#{repoName.ToLower()}"));
			attributeValues.Add(
				":sk",
				new AttributeValue($"REPO#{ownerName.ToLower()}#{repoName.ToLower()}"));
			attributeValues.Add(":status", new AttributeValue(status));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   KeyConditionExpression = "#pk = :pk AND #sk <= :sk",
									   FilterExpression = "attribute_not_exists(#data.#status) OR #data.#status = :status",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false
								   };

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<Issue>();

			foreach (var item in queryResult.Items)
			{
				if (item["Type"].S == "Issue")
				{
					response.Add(DynamoHelper.CreateFromItem<Issue>(item));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<Issue> CreateIssueAsync(
			Issue issue)
		{
			var updatedRepo = await this.IncrementRepoCount(
								  issue.OwnerName,
								  issue.RepositoryName).ConfigureAwait(false);

			issue.IssueNumber = updatedRepo.IssueAndPullRequestCount;

			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = issue.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return issue;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Issue> UpdateIssueAsync(
			Issue issue)
		{
			var updateItemRequest = new UpdateItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = issue.AsKeys(),
				ConditionExpression = "attribute_exists(PK)",
				UpdateExpression = "SET #data.#title = :title, #data.#content = :content, #data.#status = :status",
				ExpressionAttributeNames = new Dictionary<string, string>(4)
				{
					{ "#data", "Data" },
					{ "#title", "Title" },
					{ "#content", "Content" },
					{ "#status", "Status" },
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(3)
				{
					{ ":title", new AttributeValue(issue.Title) },
					{ ":content", new AttributeValue(issue.Content) },
					{ ":status", new AttributeValue(issue.Status) },
				},
				ReturnValues = ReturnValue.ALL_NEW
			};

			try
			{
				await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException e)
			{
				this._logger.LogError("Issue not found");
				throw;
			}

			return issue;
		}

		/// <inheritdoc />
		public async Task<PullRequest> CreatePullRequestAsync(
			PullRequest pullRequest)
		{
			var updatedRepo = await this.IncrementRepoCount(
								  pullRequest.OwnerName,
								  pullRequest.RepoName).ConfigureAwait(false);

			pullRequest.PullRequestNumber = updatedRepo.IssueAndPullRequestCount;

			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = pullRequest.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return pullRequest;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<PullRequest> UpdatePullRequestAsync(
			PullRequest pullRequest)
		{
			var updateItemRequest = new UpdateItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = pullRequest.AsKeys(),
				ConditionExpression = "attribute_exists(PK)",
				UpdateExpression = "SET #data.#title = :title, #data.#content = :content, #data.#status = :status",
				ExpressionAttributeNames = new Dictionary<string, string>(4)
				{
					{ "#data", "Data" },
					{ "#title", "Title" },
					{ "#content", "Content" },
					{ "#status", "Status" },
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(3)
				{
					{ ":title", new AttributeValue(pullRequest.Title) },
					{ ":content", new AttributeValue(pullRequest.Content) },
					{ ":status", new AttributeValue(pullRequest.Status) },
				},
				ReturnValues = ReturnValue.ALL_NEW
			};

			try
			{
				await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException e)
			{
				this._logger.LogError("PR not found");
				throw;
			}

			return pullRequest;
		}

		/// <inheritdoc />
		public async Task<List<PullRequest>> GetPullRequestsAsync(
			string ownerName,
			string repoName,
			string status)
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#gsi1pk", "GSI1PK");
			attributeNames.Add("#gsi1sk", "GSI1SK");
			attributeNames.Add("#data", "data");
			attributeNames.Add("#status", "status");

			var repo = new Repository() { OwnerName = ownerName, RepositoryName = repoName };

			var attributeValues = new Dictionary<string, AttributeValue>(3);
			attributeValues.Add(
				":gsi1pk",
				new AttributeValue($"REPO#{ownerName.ToLower()}#{repoName.ToLower()}"));
			attributeValues.Add(
				":gsi1sk",
				new AttributeValue($"REPO#{ownerName.ToLower()}#{repoName.ToLower()}"));
			attributeValues.Add(":status", new AttributeValue(status));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   IndexName = "GSI1",
									   KeyConditionExpression = "#gsi1pk = :gsi1pk AND #gsi1sk <= :gsi1sk",
									   FilterExpression = "attribute_not_exists(#data.#status) OR #data.#status = :status",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false
								   };

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<PullRequest>();

			foreach (var item in queryResult.Items)
			{
				if (item["Type"].S == "PullRequest")
				{
					response.Add(DynamoHelper.CreateFromItem<PullRequest>(item));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<PullRequest> GetPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber)
		{
			var pullRequest = new PullRequest()
			{
				OwnerName = ownerName,
				RepoName = repoName,
				PullRequestNumber = prNumber
			};

			var getItemRequest = new GetItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = pullRequest.AsKeys()
			};

			var getItemResponse = await this._client.GetItemAsync(getItemRequest).ConfigureAwait(false);

			if (getItemResponse.Item == null)
			{
				throw new ArgumentException("PR not found");
			}

			return DynamoHelper.CreateFromItem<PullRequest>(getItemResponse.Item);
		}

		/// <inheritdoc />
		public async Task<Repository> ForkRepository(
			string ownerName,
			string repoName,
			string forkedUserName,
			string description)
		{
			var originalRepo = new Repository()
			{
				OwnerName = ownerName,
				RepositoryName = repoName
			};

			var fork = new Repository()
			{
				OwnerName = forkedUserName,
				RepositoryName = repoName,
				ForkOwner = ownerName,
				Description = description
			};

			var transactWrite = new TransactWriteItemsRequest()
			{
				TransactItems = new List<TransactWriteItem>(2)
				{
					new TransactWriteItem()
					{
						Put = new Put()
						{
							TableName = DynamoDbConstants.TableName,
							Item = fork.AsItem(),
							ConditionExpression =  "attribute_not_exists(PK)"
						}
					},
					new TransactWriteItem()
					{
						Update = new Update()
						{
							TableName = DynamoDbConstants.TableName,
							Key = originalRepo.AsKeys(),
							ConditionExpression = "attribute_exists(PK)",
							UpdateExpression = "SET #data.#forkCount = #data.#forkCount + :inc",
							ExpressionAttributeNames = new Dictionary<string, string>(2)
							{
								{ "#data", "Data" },
								{ "#forkCount", "ForkCount" }
							},
							ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
							{
								{ ":inc", new AttributeValue() { N = "1"} }
							}
						}
					}
				}
			};

			try
			{
				await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Repo with this name already exists for account.");

				return null;
			}

			return fork;
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForksAsync(
			string ownerName,
			string repoName)
		{
			var repo = new Repository()
			{
				OwnerName = ownerName,
				RepositoryName = repoName
			};

			var queryRequest = new QueryRequest()
				{
					TableName = DynamoDbConstants.TableName,
					IndexName = "GSI2",
					KeyConditionExpression = "#gsi2pk = :gsi2pk",
					ExpressionAttributeNames = new Dictionary<string, string>(1)
					{
						{ "#gsi2pk", "GSI2PK" }
					},
					ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
					{
						{ ":gsi2pk", new AttributeValue(repo.GetGsi2Pk()) }
					},
					Limit = 26
				};

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<Repository>(queryResult.Count);

			for (int x = 0; x < queryResult.Count - 1; x++) // Run one less loop to skip the last record which will be the original repo.
			{
				response.Add(DynamoHelper.CreateFromItem<Repository>(queryResult.Items[x]));
				
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForUserAsync(
			string username)
		{
			var user = new User()
			{
				Username = username
			};

			var query = new QueryRequest()
			{
				TableName = DynamoDbConstants.TableName,
				IndexName = "GSI3",
				KeyConditionExpression = "#gsi3pk = :gsi3pk",
				ExpressionAttributeNames = new Dictionary<string, string>(1)
				{
					{ "#gsi3pk", "GSI3PK" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
				{
					{ ":gsi3pk", new AttributeValue(user.AsGsi3Pk()) }
				},
				ScanIndexForward = false,
				Limit = 11
			};

			var queryResult = await this._client.QueryAsync(query).ConfigureAwait(false);

			var response = new List<Repository>(queryResult.Count);

			foreach (var item in queryResult.Items)
			{
				response.Add(DynamoHelper.CreateFromItem<Repository>(item));
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForOrganizationAsync(
			string organizationName)
		{
			var org = new Organization()
			{
				Name = organizationName
			};

			var queryRequest = new QueryRequest()
			{
				TableName = DynamoDbConstants.TableName,
				IndexName = "GSI3",
				KeyConditionExpression = "#gsi3pk = :gsi3pk",
				ExpressionAttributeNames = new Dictionary<string, string>(1)
				{
					{ "#gsi3pk", "GSI3PK" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
				{
					{ ":gsi3pk", new AttributeValue(org.GetGsi3PK()) }
				},
				ScanIndexForward = false,
				Limit = 11
			};

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			if (queryResult.Count == 0)
			{
				throw new ArgumentException($"Organization {organizationName} does not exist");
			}

			var response = new List<Repository>(queryResult.Count = 1); // -1 to remove base item

			foreach (var item in queryResult.Items)
			{
				response.Add(DynamoHelper.CreateFromItem<Repository>(item));
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<Issue> GetIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			var issue = new Issue()
			{
				OwnerName = ownerName,
				RepositoryName = repoName,
				IssueNumber = issueNumber
			};

			var getItemResult = await this._client.GetItemAsync(
				                    new GetItemRequest()
				                    {
					                    TableName = DynamoDbConstants.TableName,
					                    Key = issue.AsKeys()
				                    });

			return DynamoHelper.CreateFromItem<Issue>(getItemResult.Item);
		}

		private async Task<Repository> IncrementRepoCount(string ownerName, string repoName)
		{
			var repo = new Repository() { OwnerName = ownerName, RepositoryName = repoName };

			var updateItemRequest = new UpdateItemRequest()
										{
											TableName = DynamoDbConstants.TableName,
											Key = repo.AsKeys(),
											ConditionExpression = "attribute_exists(PK)",
											UpdateExpression = "set #data.#count = #data.#count + :inc",
											ExpressionAttributeNames = new Dictionary<string, string>(2)
																			{
																				{ "#data", "Data" },
																				{ "#count", "IssueAndPullRequestCount" },
																			},
											ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																			{
																				{ ":inc", new AttributeValue() { N = "1"} },
																			},
											ReturnValues = ReturnValue.ALL_NEW
										};

			var updateResponse = await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);

			return DynamoHelper.CreateFromItem<Repository>(updateResponse.Attributes);
		}
	}
}
