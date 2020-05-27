using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.GitHub.Infrastructure.Extensions;

using Microsoft.Extensions.Logging;

namespace DynamoDbBook.GitHub.Infrastructure
{
    public class Interactions : IInteractions
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<Interactions> _logger;

		public Interactions(
			AmazonDynamoDBClient client,
			ILogger<Interactions> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Comment> AddCommentAsync(
			Comment comment)
		{
			var issue = new Issue()
							{
								OwnerName = comment.OwnerName,
								RepositoryName = comment.RepoName,
								IssueNumber = comment.TargetNumber
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
																				Item = comment.AsItem(),
																				ConditionExpression =
																					"attribute_not_exists(PK)"
																			}
																	},
																new TransactWriteItem()
																	{
																		ConditionCheck = new ConditionCheck()
																							 {
																								 TableName =
																									 DynamoDbConstants
																										 .TableName,
																								 Key = issue.AsKeys(),
																								 ConditionExpression =
																									 "attribute_exists(PK)"
																							 }
																	}
															}
									};

			try
			{
				var writeItemResponse = await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(ex, "Check failed");
			}

			return comment;
		}

		/// <inheritdoc />
		public async Task<Reaction> AddReactionAsync(
			Reaction reaction)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Star> StarRepoAsync(
			string ownerName,
			string repoName,
			string starringUser)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Star>> GetStargazersForRepoAsync(
			string ownerName,
			string repoName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<IssueComment>> GetCommentsForIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<IssueComment>> GetCommentsForPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber)
		{
			throw new NotImplementedException();
		}
	}
}
