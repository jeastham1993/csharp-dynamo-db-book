using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Issue : Entity
    {
		public Issue() : base()
		{
			this.Status = "Open";
		}

		public string OwnerName { get; set; }

		public string RepositoryName { get; set; }

		public string CreatorUsername { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public string Status { get; set; }

		public int IssueNumber { get; set; }

		public string PaddedIssueNumber =>
			this.IssueNumber.ToString().PadLeft(
				7,
				'0');
	}
}
