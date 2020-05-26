using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public abstract class Comment : Entity
    {
		public Comment() : base()
		{
			this.Id = Ksuid.Generate().ToString();
		}

        public string Id { get; set; }

		public string OwnerName { get; set; }

		public string RepoName { get; set; }

		public int TargetNumber { get; set; }

		public string CommentorUsername { get; set; }

		public string Content { get; set; }

		public string Reactions { get; set; }

		public abstract string TargetType { get; }

		public abstract string Type { get; }
    }
}
