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

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
	public class GetStars
	{
		private readonly IInteractions _interactions;

		public GetStars()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._interactions = serviceProvider.GetRequiredService<IInteractions>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var stars = await this._interactions.GetStargazersForRepoAsync(
				            HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
				            HttpUtility.UrlDecode(request.PathParameters["repoName"])).ConfigureAwait(false);

			if (stars != null)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonConvert.SerializeObject(stars)
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