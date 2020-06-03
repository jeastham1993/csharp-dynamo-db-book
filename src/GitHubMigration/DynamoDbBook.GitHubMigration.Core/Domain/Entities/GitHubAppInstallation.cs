using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDbBook.GitHubMigration.Core.Domain.Entities
{
    public class GitHubAppInstallation
    {
	    public GitHubAppInstallation()
	    {
			this.InstalledAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
	    }

	    public string AppOwner { get; set; }

	    public string AppName { get; set; }

	    public string RepoOwner { get; set; }

	    public string RepoName { get; set; }

	    public DateTime InstalledAt { get; set; }

	    public DateTime UpdatedAt { get; set; }
    }
}
