using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Repository : Entity
    {
        public string OwnerName { get; set; }

        public string RepositoryName { get; set; }

        public string Description { get; set; }

		public string ForkOwner { get; set; }

		public int IssueAndPullRequestCount { get; set; }

		public int StarCount { get; set; }

		public int ForkCount { get; set; }
    }
}
