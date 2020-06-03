using System;
using System.Collections.Generic;
using System.Text;

using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.GitHubMigration.Core.Domain.Entities
{
    public class Gist
    {
	    public Gist() : base()
	    {
		    this.GistId = Ksuid.Generate().ToString().Replace(
			    "/",
			    string.Empty).Replace(
			    "+",
			    string.Empty);
	    }

        public string OwnerName { get; set; }

        public string GistId { get; set; }

        public string GistTitle { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
