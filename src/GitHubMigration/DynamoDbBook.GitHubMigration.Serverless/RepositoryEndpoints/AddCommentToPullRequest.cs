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
    public class AddCommentToPullRequest
    {
	    private readonly IInteractions _interactions;

	    public AddCommentToPullRequest()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._interactions = serviceProvider.GetRequiredService<IInteractions>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    int prNumber = -1;

		    if (int.TryParse(
			        request.PathParameters["pullRequestNumber"],
			        out prNumber) == false)
		    {
			    return new APIGatewayProxyResponse
			    {
				    StatusCode = 400,
				    Body = "Invalid pr number",
			    };
		    }

		    var comment = JsonConvert.DeserializeObject<AddCommentDTO>(request.Body);

		    var result = await this._interactions.AddCommentAsync(
			                 new PullRequestComment()
			                 {
				                 CommentorUsername = comment.Username,
				                 Content = comment.Content,
				                 OwnerName = HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
				                 RepoName = HttpUtility.UrlDecode(request.PathParameters["repoName"]),
				                 TargetNumber = prNumber,
			                 });

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
