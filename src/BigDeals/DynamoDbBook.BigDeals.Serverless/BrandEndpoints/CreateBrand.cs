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
	public class CreateBrand
	{
		private readonly IBrandRepository _brandRepository;

		public CreateBrand()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var brand = JsonConvert.DeserializeObject<BrandDTO>(request.Body);

			var createdBrand = await this._brandRepository.CreateAsync(
				                   Brand.Create(
					                   brand.Name,
					                   brand.LogoUrl)).ConfigureAwait(false);

			if (createdBrand != null)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonConvert.SerializeObject(createdBrand)
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