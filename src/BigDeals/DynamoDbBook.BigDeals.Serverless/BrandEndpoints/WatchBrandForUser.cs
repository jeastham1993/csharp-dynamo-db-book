using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.DealEndpoints
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
			await this._brandRepository.WatchBrandAsync(
				            Brand.Create(
					            request.PathParameters["name"],
					            string.Empty),
				            request.PathParameters["username"]).ConfigureAwait(false);

			return new APIGatewayProxyResponse
			{
				StatusCode = 200
			};
		}
	}
}