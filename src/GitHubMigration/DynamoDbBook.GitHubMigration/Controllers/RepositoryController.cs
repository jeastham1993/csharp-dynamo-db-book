﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.ViewModels;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DynamoDbBook.GitHub.Controllers
{
	[ApiController]
	public class RepositoryController : ControllerBase
	{
		private readonly IRepoRepository _repoRepository;

		private readonly IInteractions _interactions;

		public RepositoryController(
			IRepoRepository repoRepository,
			IInteractions interactions)
		{
			this._repoRepository = repoRepository;
			this._interactions = interactions;
		}

		[HttpPost("repos")]
		public async Task<IActionResult> Create(
			Repository repository)
		{
			return new OkObjectResult(await this._repoRepository.CreateAsync(repository).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}")]
		public async Task<IActionResult> GetRepo(
			string ownerName,
			string repoName)
		{
			return new OkObjectResult(
				await this._repoRepository.GetAsync(
					ownerName,
					repoName).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/issues")]
		public async Task<IActionResult> GetRepoIssues(
			string ownerName,
			string repoName,
			string status = "Open")
		{
			return new OkObjectResult(
				await this._repoRepository.GetIssuesAsync(
					ownerName,
					repoName,
					status).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/installations")]
		public async Task<IActionResult> GetRepoInstallations(
			string ownerName,
			string repoName)
		{
			return new OkObjectResult(
				await this._repoRepository.GetInstallationsAsync(
					ownerName,
					repoName).ConfigureAwait(false));
		}

		[HttpPost("repos/{ownerName}/{repoName}/issues")]
		public async Task<IActionResult> AddIssueToRepo(
			string ownerName,
			string repoName,
			[FromBody] IssueDTO issue)
		{
			return new OkObjectResult(
				await this._repoRepository.CreateIssueAsync(
					new Issue()
					{
						Content = issue.Content,
						CreatorUsername = issue.CreatorUsername,
						OwnerName = ownerName,
						RepositoryName = repoName,
						Status = issue.Status,
						Title = issue.Title
					}).ConfigureAwait(false));
		}

		[HttpPost("repos/{ownerName}/{repoName}/issues/{issueNumber}")]
		public async Task<IActionResult> UpdateIssue(
			string ownerName,
			string repoName,
			int issueNumber,
			[FromBody] IssueDTO issue)
		{
			return new OkObjectResult(
				await this._repoRepository.UpdateIssueAsync(
					new Issue()
					{
						IssueNumber = issueNumber,
						Content = issue.Content,
						CreatorUsername = issue.CreatorUsername,
						OwnerName = ownerName,
						RepositoryName = repoName,
						Status = issue.Status,
						Title = issue.Title
					}).ConfigureAwait(false));
		}

		[HttpPost("repos/{ownerName}/{repoName}/code")]
		public async Task<IActionResult> UpdateCodeOfConduct(
			string ownerName,
			string repoName,
			[FromBody] CodeOfConduct code)
		{
			return new OkObjectResult(
				await this._repoRepository.UpdateCodeOfConductAsync(
					ownerName,
					repoName,
					code).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/issues/{issueNumber}/comments")]
		public async Task<IActionResult> GetCommentsForIssue(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			return new OkObjectResult(
				await this._interactions.GetCommentsForIssueAsync(
					ownerName,
					repoName,
					issueNumber).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/issues/{issueNumber}")]
		public async Task<IActionResult> GetIssue(
			string ownerName,
			string repoName,
			int issueNumber)
		{
			return new OkObjectResult(
				await this._repoRepository.GetIssueAsync(
					ownerName,
					repoName,
					issueNumber).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/pulls/{prNumber}")]
		public async Task<IActionResult> GetPullRequest(
			string ownerName,
			string repoName,
			int prNumber)
		{
			return new OkObjectResult(
				await this._repoRepository.GetPullRequestAsync(
					ownerName,
					repoName,
					prNumber).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/pulls")]
		public async Task<IActionResult> GetRepoPullRequests(
			string ownerName,
			string repoName,
			string status = "Open")
		{
			return new OkObjectResult(
				await this._repoRepository.GetPullRequestsAsync(
					ownerName,
					repoName,
					status).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/stars")]
		public async Task<IActionResult> GetStargazers(
			string ownerName,
			string repoName)
		{
			return new OkObjectResult(
				await this._interactions.GetStargazersForRepoAsync(
					ownerName,
					repoName).ConfigureAwait(false));
		}

		[HttpGet("repos/{ownerName}/{repoName}/pulls/{pullRequestNumber}/comments")]
		public async Task<IActionResult> GetCommentsForPullRequest(
			string ownerName,
			string repoName,
			int pullRequestNumber)
		{
			return new OkObjectResult(
				await this._interactions.GetCommentsForPullRequestAsync(
					ownerName,
					repoName,
					pullRequestNumber).ConfigureAwait(false));
		}

		[HttpPost("repos/{ownerName}/{repoName}/pulls")]
		public async Task<IActionResult> AddPullRequestToRepo(
			string ownerName,
			string repoName,
			[FromBody] PullRequestDTO pullRequest)
		{
			return new OkObjectResult(
				await this._repoRepository.CreatePullRequestAsync(
					new PullRequest()
					{
						Content = pullRequest.Content,
						CreatorUsername = pullRequest.CreatorUsername,
						OwnerName = ownerName,
						PullRequestNumber = pullRequest.PullRequestNumber,
						RepoName = repoName,
						Title = pullRequest.Title,
					}).ConfigureAwait(false));
		}

		[HttpPut("repos/{ownerName}/{repoName}/pulls/{prNumber}")]
		public async Task<IActionResult> UpdatePullRequest(
			string ownerName,
			string repoName,
			int prNumber,
			[FromBody] PullRequestDTO pullRequest)
		{
			return new OkObjectResult(
				await this._repoRepository.UpdatePullRequestAsync(
					new PullRequest()
					{
						Content = pullRequest.Content,
						CreatorUsername = pullRequest.CreatorUsername,
						OwnerName = ownerName,
						PullRequestNumber = prNumber,
						RepoName = repoName,
						Title = pullRequest.Title,
					}).ConfigureAwait(false));
		}
	}
}