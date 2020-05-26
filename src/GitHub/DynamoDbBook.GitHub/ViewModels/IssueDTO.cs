using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class IssueDTO
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string Status { get; set; }

		public string CreatorUsername { get; set; }
    }
}
