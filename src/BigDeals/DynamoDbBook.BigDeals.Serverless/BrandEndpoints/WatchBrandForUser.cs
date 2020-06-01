using System;
using System.Threading.Tasks;
using System.Web;

using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.BrandEndpoints
{
	public class WatchBrandForUser
	{
		private readonly IBrandRepository _brandRepository;

		public WatchBrandForUser()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			try
			{
				context.Logger.Log(request.PathParameters["name"]);

				await this._brandRepository.WatchBrandAsync(
					HttpUtility.UrlDecode(request.PathParameters["name"]),
					HttpUtility.UrlDecode(request.PathParameters["username"])).ConfigureAwait(false);

				return new APIGatewayProxyResponse
				{
					StatusCode = 200
				};
			}
			catch (ConditionalCheckFailedException)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 500,
					Body = $"Brand {request.PathParameters["name"]} not found"
				};
			}
			catch (Exception ex)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 500,
					Body = $"{request.PathParameters["name"]}{Environment.NewLine}{JsonConvert.SerializeObject(ex)}"
				};
			}
		}
	}
}