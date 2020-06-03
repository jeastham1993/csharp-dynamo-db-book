using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.AppEndpoints
{
    public class InstallApp
    {
	    private readonly IAppRepository _appRepository;

	    public InstallApp()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._appRepository = serviceProvider.GetRequiredService<IAppRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var gitHubAppInstallation = JsonConvert.DeserializeObject<GitHubAppInstallation>(request.Body);
		    gitHubAppInstallation.AppOwner = HttpUtility.UrlDecode(request.PathParameters["appOwner"]);
		    gitHubAppInstallation.AppName = HttpUtility.UrlDecode(request.PathParameters["appName"]);

		    var created = await this._appRepository.InstallAppAsync(gitHubAppInstallation).ConfigureAwait(false);

		    if (created != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(created)
			    };
		    }
		    else
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 400
			    };
		    }
	    }
    }
}
