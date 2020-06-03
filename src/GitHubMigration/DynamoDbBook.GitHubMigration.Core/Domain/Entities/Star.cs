using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Star
    {
		public string OwnerName { get; set; }

		public string RepoName { get; set; }

		public string Username { get; set; }

		public DateTime StarredAt { get; set; }
    }
}
