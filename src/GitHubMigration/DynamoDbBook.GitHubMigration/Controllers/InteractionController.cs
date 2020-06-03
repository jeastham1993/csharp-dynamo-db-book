using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace DynamoDbBook.GitHub.Controllers
{
	[ApiController]
	public class InteractionController : ControllerBase
	{
		private readonly IInteractions _interactions;

		public InteractionController(
			IInteractions interactions)
		{
			this._interactions = interactions;
		}

		[HttpPost("repos/{ownerName}/{repoName}/issues/{issueNumber}/comments")]
		public async Task<IActionResult> AddCommentToIssue(
			string ownerName,
			string repoName,
			int issueNumber,
			[FromBody] AddCommentDTO comment)
		{
			return new OkObjectResult(
				await this._interactions.AddCommentAsync(
					new IssueComment()
						{
							CommentorUsername = comment.Username,
							Content = comment.Content,
							OwnerName = ownerName,
							RepoName = repoName,
							TargetNumber = issueNumber,
						}));
		}

		[HttpPost("repos/{ownerName}/{repoName}/pulls/{prNumber}/comments")]
		public async Task<IActionResult> AddCommentToPullRequest(
			string ownerName,
			string repoName,
			int prNumber,
			[FromBody] AddCommentDTO comment)
		{
			return new OkObjectResult(
				await this._interactions.AddCommentAsync(
					new PullRequestComment()
						{
							CommentorUsername = comment.Username,
							Content = comment.Content,
							OwnerName = ownerName,
							RepoName = repoName,
							TargetNumber = prNumber
						}));
		}

		[HttpPost("repos/{ownerName}/{repoName}/star")]
		public async Task<IActionResult> AddStarToRepo(
			string ownerName,
			string repoName,
			[FromBody] AddStarDTO star)
		{
			return new OkObjectResult(
				await this._interactions.StarRepoAsync(
					ownerName,
					repoName,
					star.Username));
		}
	}
}