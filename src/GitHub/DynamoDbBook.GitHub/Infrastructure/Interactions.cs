using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;

namespace DynamoDbBook.GitHub.Infrastructure
{
    public class Interactions : IInteractions
    {
		/// <inheritdoc />
		public async Task<IssueComment> AddCommentToIssueAsync(
			IssueComment comment)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<PullRequestComment> AddCommentToPullRequestAsync(
			PullRequestComment comment)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<CommentReaction> AddReactionToCommentAsync(
			CommentReaction reaction)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<CommentReaction> AddReactionToIssueAsync(
			IssueReaction reaction)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<CommentReaction> AddReactionToPullRequestAsync(
			PullRequestReaction reaction)
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
