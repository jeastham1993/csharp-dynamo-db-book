using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.BrandEndpoints
{
	public class GetBrand
	{
		private readonly IBrandRepository _brandRepository;

		public GetBrand()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var brand = await this._brandRepository.GetBrandAsync(HttpUtility.UrlDecode(request.PathParameters["name"]))
				             .ConfigureAwait(false);

			return new APIGatewayProxyResponse
			{
				StatusCode = 200,
				Body = JsonConvert.SerializeObject(brand)
			};
		}
	}
}