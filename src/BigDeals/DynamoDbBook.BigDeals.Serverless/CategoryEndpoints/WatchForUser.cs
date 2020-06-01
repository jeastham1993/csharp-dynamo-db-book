using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.BigDeals.Serverless.CategoryEndpoints
{
	public class WatchForUser
	{
		private readonly ICategoryRepository _categoryRepository;

		public WatchForUser()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			await this._categoryRepository.WatchCategoryAsync(
				HttpUtility.UrlDecode(request.PathParameters["name"]),
				HttpUtility.UrlDecode(request.PathParameters["username"])).ConfigureAwait(false);

			return new APIGatewayProxyResponse
			{
				StatusCode = 200
			};
		}
	}
}