using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure.Extensions;

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

			var data = getItem.Item.FirstOrDefault(p => p.Key == "Data");

			return JsonConvert.DeserializeObject<Repository>(Document.FromAttributeMap(data.Value.M).ToJson());
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
					var responseData = item.FirstOrDefault(p => p.Key == "Data");

					response.Add(
						JsonConvert.DeserializeObject<Issue>(Document.FromAttributeMap(responseData.Value.M).ToJson()));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<Issue> CreateIssueAsync(
			Issue issue)
		{
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
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<PullRequest> CreatePullRequestAsync(
			PullRequest pullRequest)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<PullRequest> UpdatePullRequestAsync(
			PullRequest pullRequest)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<PullRequest>> GetPullRequestsAsync(
			string ownerName,
			string repoName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<PullRequest> GetPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Repository> ForkRepository(
			string ownerName,
			string repoName,
			string forkedUserName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForksAsync(
			string ownerName,
			string repoName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForUserAsync(
			string username)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Repository>> GetForOrganizationAsync(
			string organizationName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Issue> GetIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			throw new NotImplementedException();
		}
	}
}
