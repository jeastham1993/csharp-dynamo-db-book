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

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
    public class GetPullRequest
    {
	    private readonly IRepoRepository _repoRepository;

	    public GetPullRequest()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._repoRepository = serviceProvider.GetRequiredService<IRepoRepository>();
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
					Body = "Invalid issue number",
			    };
		    }

		    var issue = await this._repoRepository.GetPullRequestAsync(
			               HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
			               HttpUtility.UrlDecode(request.PathParameters["repoName"]),
			               pullRequestNumber).ConfigureAwait(false);

		    if (issue != null)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 200,
				    Body = JsonConvert.SerializeObject(issue)
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
