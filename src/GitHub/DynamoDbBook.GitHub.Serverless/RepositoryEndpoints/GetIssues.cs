using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
    public class GetIssues
    {
	    private readonly IRepoRepository _repoRepository;

	    public GetIssues()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._repoRepository = serviceProvider.GetRequiredService<IRepoRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var status = "Open";

		    if (request.QueryStringParameters.TryGetValue(
			    "status",
			    out string parsedStatus))
		    {
			    status = parsedStatus;
		    }

		    var issues = await this._repoRepository.GetIssuesAsync(
			               HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
			               HttpUtility.UrlDecode(request.PathParameters["repoName"]),
			               status).ConfigureAwait(false);

		    if (issues != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(issues)
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
