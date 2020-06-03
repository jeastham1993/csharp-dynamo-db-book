using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities.Reactions
{
    public class IssueReaction : Reaction
	{
		/// <inheritdoc />
		public override string Type => "Issue";
	}
}
