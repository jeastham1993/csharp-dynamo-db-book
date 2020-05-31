using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class PullRequestDTO
    {
        public string CreatorUsername { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int PullRequestNumber { get; set; }
    }
}
