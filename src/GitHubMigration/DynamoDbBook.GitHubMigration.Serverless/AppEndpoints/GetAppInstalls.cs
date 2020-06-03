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
    public class GetAppInstalls
    {
	    private readonly IAppRepository _appRepository;

	    public GetAppInstalls()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._appRepository = serviceProvider.GetRequiredService<IAppRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var appInstalls = await this._appRepository.GetAppInstallations(
			                  HttpUtility.UrlDecode(request.PathParameters["appOwner"]),
			                  HttpUtility.UrlDecode(request.PathParameters["appName"])).ConfigureAwait(false);

		    if (appInstalls != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(appInstalls)
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
