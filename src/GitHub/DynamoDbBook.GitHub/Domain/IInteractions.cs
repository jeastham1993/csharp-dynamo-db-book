using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;

namespace DynamoDbBook.GitHub.Domain
{
	public interface IInteractions
	{
		Task<Comment> AddCommentAsync(
			Comment comment);

		Task<Reaction> AddReactionAsync(
			Reaction reaction);

		Task<Star> StarRepoAsync(
			string ownerName,
			string repoName,
			string starringUser);

		Task<List<Star>> GetStargazersForRepoAsync(
			string ownerName,
			string repoName);

		Task<List<IssueComment>> GetCommentsForIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber);

		Task<List<IssueComment>> GetCommentsForPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber);
	}
}
