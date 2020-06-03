using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain.Entities.Reactions;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Issue : Entity
    {
		public Issue() : base()
		{
			this.Status = "Open";

			this.Reactions = new Dictionary<string, int>(8);

			foreach (var reactionType in ReactionHelpers.ReactionTypes)
			{
				this.Reactions.Add(
					reactionType.Key,
					0);
			}
		}

		public string OwnerName { get; set; }

		public string RepositoryName { get; set; }

		public string CreatorUsername { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string Status { get; set; }

		public Dictionary<string, int> Reactions { get; set; }

		public int IssueNumber { get; set; }

		public string PaddedIssueNumber =>
			this.IssueNumber.ToString().PadLeft(
				7,
				'0');

		public string PaddedIssueNumberDifference
		{
			get
			{
				var difference = 9999999 - this.IssueNumber;

				return difference.ToString().PadLeft(
					7,
					'0');
			}
		}
    }
}
