using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHubMigration.Core.Domain.Entities
{
    public interface IAppRepository
    {
	    Task<GitHubApp> CreateAppAsync(
		    GitHubApp app);

	    Task<GitHubApp> InstallAppAsync(
		    GitHubAppInstallation appInstall);

	    Task<IEnumerable<GitHubAppInstallation>> GetAppInstallations(
		    string appOwner,
		    string appName);
    }
}
