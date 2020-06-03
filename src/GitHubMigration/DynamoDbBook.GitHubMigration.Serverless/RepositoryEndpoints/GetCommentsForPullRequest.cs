using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
    public class GetCommentsForPullRequest
    {
	    private readonly IInteractions _interactions;

	    public GetCommentsForPullRequest()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._interactions = serviceProvider.GetRequiredService<IInteractions>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    int pullRequestNumber = -1;

		    if (int.TryParse(
			        request.PathParameters["pullRequestNumber"],
			        out pullRequestNumber) == false)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 400,
				    Body = "Invalid pr number",
			    };
		    }

		    var result = await this._interactions.GetCommentsForPullRequestAsync(
			                 HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
			                 HttpUtility.UrlDecode(request.PathParameters["repoName"]),
			                 pullRequestNumber);

		    if (result != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(result)
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
