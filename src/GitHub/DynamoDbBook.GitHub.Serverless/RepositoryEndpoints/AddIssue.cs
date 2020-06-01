using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
    public class AddIssue
    {
	    private readonly IRepoRepository _repoRepository;

	    public AddIssue()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._repoRepository = serviceProvider.GetRequiredService<IRepoRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var issue = JsonConvert.DeserializeObject<IssueDTO>(request.Body);

		    var created = await this._repoRepository.CreateIssueAsync(
			                  new Issue()
			                  {
				                  Content = issue.Content,
				                  CreatorUsername = issue.CreatorUsername,
				                  OwnerName = HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
				                  RepositoryName = HttpUtility.UrlDecode(request.PathParameters["repoName"]),
				                  Status = issue.Status,
				                  Title = issue.Title
			                  }).ConfigureAwait(false);

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
