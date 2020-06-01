using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.BigDeals.Serverless.BrandEndpoints
{
	public class LikeBrandForUser
	{
		private readonly IBrandRepository _brandRepository;

		public LikeBrandForUser()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			await this._brandRepository.LikeBrandAsync(
				HttpUtility.UrlDecode(request.PathParameters["name"]),
				HttpUtility.UrlDecode(request.PathParameters["username"])).ConfigureAwait(false);

			return new APIGatewayProxyResponse
			{
				StatusCode = 200
			};
		}
	}
}