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
using DynamoDbBook.SharedKernel;

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
			Dictionary<string, AttributeValue> updateKeys = null;

			switch (comment.GetType().Name)
			{
				case "IssueComment":
					updateKeys = new Issue()
					{
						OwnerName = comment.OwnerName,
						RepositoryName = comment.RepoName,
						IssueNumber = comment.TargetNumber
					}.AsKeys();
					break;
				case "PullRequestComment":
					updateKeys = new PullRequest()
					{
						OwnerName = comment.OwnerName,
						RepoName = comment.RepoName,
						PullRequestNumber = comment.TargetNumber
					}.AsKeys();
					break;
			}

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
							ConditionExpression = "attribute_not_exists(PK)"
						}
					},
					new TransactWriteItem()
					{
						ConditionCheck = new ConditionCheck()
						{
							TableName = DynamoDbConstants.TableName,
							Key = updateKeys,
							ConditionExpression = "attribute_exists(PK)"
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
				this._logger.LogError(
					ex,
					"Check failed");
			}

			return comment;
		}

		/// <inheritdoc />
		public async Task<Reaction> AddReactionAsync(
			Reaction reaction)
		{
			Dictionary<string, AttributeValue> updateKeys = null;

			switch (reaction.GetType().Name)
			{
				case "CommentReaction":
					updateKeys = new IssueComment()
					{
						OwnerName = reaction.OwnerName,
						RepoName = reaction.RepoName,
						Id = reaction.Id,
						TargetNumber = reaction.TargetNumber
					}.AsKeys();
					break;
				case "IssueReaction":
					updateKeys = new Issue()
					{
						OwnerName = reaction.OwnerName,
						RepositoryName = reaction.RepoName,
						IssueNumber = reaction.TargetNumber,
					}.AsKeys();
					break;
				case "PullRequestReaction":
					updateKeys = new PullRequest()
					{
						OwnerName = reaction.OwnerName,
						RepoName = reaction.RepoName,
						PullRequestNumber = reaction.TargetNumber,
					}.AsKeys();
					break;
				default:
					throw new ArgumentException($"Invalid reaction type {reaction.GetType().Name}");
			}

			var transactWrite = new TransactWriteItemsRequest()
			{
				TransactItems = new List<TransactWriteItem>(2)
				{
					new TransactWriteItem()
					{
						Update = new Update()
						{
							TableName = DynamoDbConstants.TableName,
							Key = reaction.AsKeys(),
							ConditionExpression = "attribute_not_exists(#data.#reaction) OR NOT contains(#data.#reaction, :reaction)",
							UpdateExpression = "ADD #reaction :reactionSet",
							ExpressionAttributeNames = new Dictionary<string, string>(2)
							{
								{ "#data", "Data" },
								{ "#reaction", "Reaction" }
							},
							ExpressionAttributeValues = new Dictionary<string,AttributeValue>(2)
							{
								{ ":reaction", new AttributeValue(reaction.ReactionType) },
								{ ":reactionSet", new AttributeValue(new List<string>(1) { reaction.ReactionType }) }
							}
						}
					},
					new TransactWriteItem()
					{
						Update = new Update()
						{
							TableName = DynamoDbConstants.TableName,
							Key = updateKeys,
							ConditionExpression = "attribute_exists(PK)",
							UpdateExpression = "SET #data.#reactions.#reaction = #data.#reactions.#reaction + :inc",
							ExpressionAttributeNames = new Dictionary<string, string>(3)
							{
								{ "#data", "Data" },
								{ "#reactions", "Reactions" },
								{ "#reaction", reaction.ReactionType }
							},
							ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
							{
								{ ":inc", new AttributeValue() { N = "1" } }
							}
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
				this._logger.LogError(
					ex,
					"Check failed");
			}

			return reaction;
		}

		/// <inheritdoc />
		public async Task<Star> StarRepoAsync(
			string ownerName,
			string repoName,
			string starringUser)
		{
			var star = new Star()
			{
				OwnerName = ownerName,
				RepoName = repoName,
				StarredAt = DateTime.Now,
				Username = starringUser
			};

			var repo = new Repository()
			{
				OwnerName = ownerName,
				RepositoryName = repoName
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
							ConditionExpression = "attribute_not_exists(PK)",
							Item = star.AsItem()
						}
					},
					new TransactWriteItem()
					{
						Update = new Update()
						{
							TableName = DynamoDbConstants.TableName,
							Key = repo.AsKeys(),
							ConditionExpression = "attribute_exists(PK)",
							UpdateExpression = "SET #data.#starCount = #data.#starCount + :inc",
							ExpressionAttributeNames = new Dictionary<string, string>(3)
							{
								{ "#data", "Data" },
								{ "#starCount", "StarCount" },
							},
							ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
							{
								{ ":inc", new AttributeValue() { N = "1" } }
							}
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
				this._logger.LogError(
					ex,
					"Check failed");
			}

			return star;
		}

		/// <inheritdoc />
		public async Task<List<Star>> GetStargazersForRepoAsync(
			string ownerName,
			string repoName)
		{
			var repo = new Repository()
			{
				OwnerName = ownerName,
				RepositoryName = repoName
			};

			var query = new QueryRequest()
			{
				TableName = DynamoDbConstants.TableName,
				KeyConditionExpression = "#pk = :pk AND #sk >= :sk",
				ExpressionAttributeNames = new Dictionary<string, string>(2)
				{
					{ "#pk", "PK" },
					{ "#sk", "SK" }
				},
				ExpressionAttributeValues = new Dictionary<string, AttributeValue>(2)
				{
					{ ":pk", new AttributeValue(repo.GetPk()) },
					{ ":sk", new AttributeValue(repo.GetSk()) }
				},
				Limit = 26
			};

			var queryResult = await this._client.QueryAsync(query).ConfigureAwait(false);

			var response = new List<Star>(queryResult.Count);

			foreach (var item in queryResult.Items)
			{
				if (item["Type"].S == "Star")
				{
					response.Add(DynamoHelper.CreateFromItem<Star>(item));
				}
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<List<IssueComment>> GetCommentsForIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			Comment issueComment = new IssueComment()
			{
				OwnerName = ownerName,
				RepoName = repoName,
				TargetNumber = issueNumber
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
					{ ":pk", new AttributeValue(issueComment.GetPk()) }
				}
			};

			var result = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<IssueComment>(result.Count);

			foreach (var item in result.Items)
			{
				response.Add(DynamoHelper.CreateFromItem<IssueComment>(item));
			}

			return response;
		}

		/// <inheritdoc />
		public async Task<List<PullRequestComment>> GetCommentsForPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber)
		{
			Comment comment = new PullRequestComment()
			{
				OwnerName = ownerName,
				RepoName = repoName,
				TargetNumber = prNumber
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
					{ ":pk", new AttributeValue(comment.GetPk()) }
				}
			};

			var result = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var response = new List<PullRequestComment>(result.Count);

			foreach (var item in result.Items)
			{
				response.Add(DynamoHelper.CreateFromItem<PullRequestComment>(item));
			}

			return response;
		}
	}
}