using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities.Reactions
{
    public abstract class Reaction
    {
		protected Reaction()
			: base()
		{
		}

        public string OwnerName { get; set; }

		public string RepoName { get; set; }
		
		public string ReactionType { get; set; }

		public string Id { get; set; }

		public string ReactingUsername { get; set; }

        public abstract string Type { get; }
    }
}
