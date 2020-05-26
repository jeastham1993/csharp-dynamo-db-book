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
	[Route("[controller]")]
	public class InteractionController : ControllerBase
	{
		private readonly IInteractions _interactions;

		public InteractionController(
			IInteractions interactions)
		{
			this._interactions = interactions;
		}

		[HttpPost("{ownerName}/{repoName}/issue/{issueNumber}")]
		public async Task<IActionResult> AddCommentToIssue(
			string ownerName,
			string repoName,
			int issueNumber,
			[FromBody] AddCommentDTO comment)
		{
			return new OkObjectResult(
				await this._interactions.AddCommentToIssueAsync(
					new IssueComment()
						{
							CommentorUsername = comment.Username,
							Content = comment.Content,
							OwnerName = ownerName,
							RepoName = repoName,
							TargetNumber = issueNumber
						}));
		}

		[HttpPost("{ownerName}/{repoName}/pr/{prNumber}")]
		public async Task<IActionResult> AddCommentToPullRequest(
			string ownerName,
			string repoName,
			int prNumber,
			[FromBody] AddCommentDTO comment)
		{
			return new OkObjectResult(
				await this._interactions.AddCommentToPullRequestAsync(
					new PullRequestComment()
						{
							CommentorUsername = comment.Username,
							Content = comment.Content,
							OwnerName = ownerName,
							RepoName = repoName,
							TargetNumber = prNumber
						}));
		}

		[HttpPost("{ownerName}/{repoName}/comment/{commentId}")]
		public async Task<IActionResult> AddReactionToComment(
			string ownerName,
			string repoName,
			string commentId,
			[FromBody] AddReactionDTO reaction)
		{
			return new OkObjectResult(
				await this._interactions.AddReactionToCommentAsync(
					new CommentReaction()
						{
							OwnerName = ownerName,
							Id = commentId,
							ReactingUsername = reaction.Username,
							RepoName = repoName,
							ReactionType = reaction.Reaction
						}));
		}

		[HttpPost("{ownerName}/{repoName}/issue/{issueId}")]
		public async Task<IActionResult> AddReactionToIssue(
			string ownerName,
			string repoName,
			string issueId,
			[FromBody] AddReactionDTO reaction)
		{
			return new OkObjectResult(
				await this._interactions.AddReactionToIssueAsync(
					new IssueReaction()
						{
							OwnerName = ownerName,
							Id = issueId,
							ReactingUsername = reaction.Username,
							RepoName = repoName,
							ReactionType = reaction.Reaction
						}));
		}

		[HttpPost("{ownerName}/{repoName}/pr/{pullRequestId}")]
		public async Task<IActionResult> AddReactionToPullRequest(
			string ownerName,
			string repoName,
			string pullRequestId,
			[FromBody] AddReactionDTO reaction)
		{
			return new OkObjectResult(
				await this._interactions.AddReactionToPullRequestAsync(
					new PullRequestReaction()
						{
							OwnerName = ownerName,
							Id = pullRequestId,
							ReactingUsername = reaction.Username,
							RepoName = repoName,
							ReactionType = reaction.Reaction
						}));
		}

		[HttpPost("{ownerName}/{repoName}/star")]
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