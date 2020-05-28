using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public abstract class Comment : Entity
    {
		public Comment() : base()
		{
			this.Id = Ksuid.Generate().ToString().Replace(
				"/",
				string.Empty).Replace(
				"+",
				string.Empty);

			this.Reactions = new Dictionary<string, int>(8);

			foreach (var reactionType in ReactionHelpers.ReactionTypes)
			{
				this.Reactions.Add(
					reactionType.Key,
					0);
			}
		}

        public string Id { get; set; }

		public string OwnerName { get; set; }

		public string RepoName { get; set; }

		public int TargetNumber { get; set; }

		public string CommentorUsername { get; set; }

		public string Content { get; set; }

		public Dictionary<string, int> Reactions { get; set; }

		public abstract string TargetType { get; }

		public abstract string Type { get; }

		public string PaddedTargetNumber =>
			this.TargetNumber.ToString().PadLeft(
				7,
				'0');
    }
}
