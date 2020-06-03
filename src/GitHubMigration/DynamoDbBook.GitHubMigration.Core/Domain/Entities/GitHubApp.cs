using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDbBook.GitHubMigration.Core.Domain.Entities
{
    public class GitHubApp
    {
	    public GitHubApp()
	    {
			this.CreatedAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
	    }

	    public string AppOwner { get; set; }

	    public string AppName { get; set; }

	    public string Description { get; set; }

	    public DateTime CreatedAt { get; set; }

	    public DateTime UpdatedAt { get; set; }
    }
}
