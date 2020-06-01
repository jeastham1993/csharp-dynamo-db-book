using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.CategoryEndpoints
{
	public class Get
	{
		private readonly ICategoryRepository _categoryRepository;

		public Get()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var category = await this._categoryRepository.GetCategoryAsync(HttpUtility.UrlDecode(request.PathParameters["name"]))
				               .ConfigureAwait(false);

			if (category != null)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonConvert.SerializeObject(category)
				};
			}
			else
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 404,
					Body = "Deal not found"
				};
			}
		}
	}
}