using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.CategoryEndpoints
{
	public class Update
	{
		private readonly ICategoryRepository _categoryRepository;

		public Update()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var category = JsonConvert.DeserializeObject<CategoryDTO>(request.Body);

			var updatedCategory = await this._categoryRepository.CreateOrUpdateAsync(
				                      Category.Create(
					                      category.CategoryName,
					                      category.FeaturedDeals));

			if (updatedCategory != null)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonConvert.SerializeObject(updatedCategory)
				};
			}
			else
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 400,
					Body = "Order exists"
				};
			}
		}
	}
}