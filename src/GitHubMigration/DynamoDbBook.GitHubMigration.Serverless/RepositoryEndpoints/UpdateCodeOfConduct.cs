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
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.RepositoryEndpoints
{
	public class UpdateCodeOfConduct
	{
		private readonly IRepoRepository _repoRepository;

		public UpdateCodeOfConduct()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._repoRepository = serviceProvider.GetRequiredService<IRepoRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			{
				var codeOfConduct = JsonConvert.DeserializeObject<CodeOfConduct>(request.Body);

				var created = await this._repoRepository.UpdateCodeOfConductAsync(
					              HttpUtility.UrlDecode(request.PathParameters["ownerName"]),
					              HttpUtility.UrlDecode(request.PathParameters["repoName"]),
					              codeOfConduct).ConfigureAwait(false);

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
}