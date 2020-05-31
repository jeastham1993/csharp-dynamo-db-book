using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class IssueComment : Comment
	{
		/// <inheritdoc />
		public override string Type => "IssueComment";

		/// <inheritdoc />
		public override string TargetType => "Issue";
	}
}
