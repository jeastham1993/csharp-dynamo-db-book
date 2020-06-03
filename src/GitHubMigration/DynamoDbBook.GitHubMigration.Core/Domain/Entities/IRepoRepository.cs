using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public interface IRepoRepository
	{
		Task<Repository> GetAsync(
			string ownerName,
			string repoName);

		Task<Repository> CreateAsync(
			Repository repo);

		Task<List<GitHubAppInstallation>> GetInstallationsAsync(
			string ownerName,
			string repoName);

		Task<List<Issue>> GetIssuesAsync(
			string ownerName,
			string repoName,
			string status);

		Task<Issue> CreateIssueAsync(
			Issue issue);

		Task<Issue> UpdateIssueAsync(
			Issue issue);

		Task<Repository> UpdateCodeOfConductAsync(
			string ownerName,
			string repoName,
			CodeOfConduct codeOfConduct);

		Task<PullRequest> CreatePullRequestAsync(
			PullRequest pullRequest);

		Task<PullRequest> UpdatePullRequestAsync(
			PullRequest pullRequest);

		Task<List<PullRequest>> GetPullRequestsAsync(
			string ownerName,
			string repoName,
			string status);

		Task<PullRequest> GetPullRequestAsync(
			string ownerName,
			string repoName,
			int prNumber);

		Task<Repository> ForkRepository(
			string ownerName,
			string repoName,
			string forkedUserName,
			string description);

		Task<List<Repository>> GetForksAsync(
			string ownerName,
			string repoName);

		Task<List<Repository>> GetForUserAsync(
			string username);

		Task<List<Repository>> GetForOrganizationAsync(
			string organizationName);

		Task<Issue> GetIssueAsync(
			string ownerName,
			string repoName,
			int issueNumber);
	}
}
