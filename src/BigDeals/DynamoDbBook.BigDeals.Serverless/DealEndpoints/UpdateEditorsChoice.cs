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
    public class UpdateEditorsChoice
    {
	    private readonly IDealRepository _dealRepository;

	    public UpdateEditorsChoice()
	    {
		    var serviceCollection = new ServiceCollection()
			    .AddLogging()
			    .ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._dealRepository = serviceProvider.GetRequiredService<IDealRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var deals = JsonConvert.DeserializeObject<List<Deal>>(request.Body);

		    await this._dealRepository.UpdateEditorsChoiceAsync(deals).ConfigureAwait(false);

			return new APIGatewayProxyResponse { StatusCode = 200 };
	    }
    }
}
