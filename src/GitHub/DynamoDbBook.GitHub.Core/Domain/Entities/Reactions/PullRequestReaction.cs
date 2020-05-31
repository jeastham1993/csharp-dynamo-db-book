using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities.Reactions
{
    public class PullRequestReaction : Reaction
	{
		/// <inheritdoc />
		public override string Type => "PR";
	}
}
