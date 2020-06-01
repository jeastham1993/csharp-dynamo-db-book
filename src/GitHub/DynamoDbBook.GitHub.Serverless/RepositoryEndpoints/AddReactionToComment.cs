using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
    public class AddReactionToComment
    {
	    private readonly IInteractions _interactions;

	    public AddReactionToComment()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._interactions = serviceProvider.GetRequiredService<IInteractions>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var reaction = JsonConvert.DeserializeObject<AddReactionDTO>(request.Body);

		    var result = await this._interactions.AddReactionAsync(
			                 new CommentReaction()
			                 {
				                 OwnerName = HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
				                 Id = HttpUtility.UrlDecode(request.PathParameters["commentId"]),
				                 ReactingUsername = reaction.Username,
				                 RepoName = HttpUtility.UrlDecode(request.PathParameters["repoName"]),
				                 ReactionType = reaction.Reaction,
								 TargetNumber = reaction.TargetNumber
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
