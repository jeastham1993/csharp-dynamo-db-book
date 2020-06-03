using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.UserEndpoints
{
    public class GetOrganizationRepos
    {
	    private readonly IRepoRepository _repoRepository;

	    public GetOrganizationRepos()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._repoRepository = serviceProvider.GetRequiredService<IRepoRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var repos = await this._repoRepository.GetForOrganizationAsync(
			               HttpUtility.UrlDecode(request.PathParameters["organizationName"])).ConfigureAwait(false);

		    if (repos != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(repos)
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
