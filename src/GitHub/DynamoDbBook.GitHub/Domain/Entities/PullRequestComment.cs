using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class PullRequestComment : Comment
    {
		/// <inheritdoc />
		public override string Type => "PRComment";

		/// <inheritdoc />
		public override string TargetType => "PullRequest";
    }
}
