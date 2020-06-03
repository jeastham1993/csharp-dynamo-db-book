using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities.Reactions
{
    public class CommentReaction : Reaction
	{
		/// <inheritdoc />
		public override string Type => "Comment";
	}
}
