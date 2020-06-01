using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.BrandEndpoints
{
    public class ListBrands
    {
	    private readonly IBrandRepository _brandRepository;

	    public ListBrands()
	    {
		    var serviceCollection = new ServiceCollection()
			    .AddLogging()
			    .ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var brands = await this._brandRepository.ListBrandsAsync().ConfigureAwait(false);

		    return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(brands)};
	    }
    }
}
