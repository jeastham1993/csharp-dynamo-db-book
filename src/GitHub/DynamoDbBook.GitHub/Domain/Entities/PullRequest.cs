using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
	public class PullRequest : Entity
	{
		public PullRequest() : base()
		{
			this.Status = "Open";
		}

		public string OwnerName { get; set; }

		public string RepoName { get; set; }

		public string CreatorUsername { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string Status { get; set; }

		public int PullRequestNumber { get; set; }

		public string Reactions { get; set; }

		public string PaddedPullRequestNumber =>
			this.PullRequestNumber.ToString().PadLeft(
				7,
				'0');
    }
}
