using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Core.Domain.Request;
using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.DealEndpoints
{
    class HotDealBlast
    {
	    private readonly SendHotDealInteractor _hotDealInteractor;

	    public HotDealBlast()
	    {
		    var serviceCollection = new ServiceCollection()
			    .AddLogging()
			    .ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._hotDealInteractor = serviceProvider.GetRequiredService<SendHotDealInteractor>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var hotDealRequest = JsonConvert.DeserializeObject<SendHotDealRequest>(request.Body);

		    await this._hotDealInteractor.Handle(hotDealRequest).ConfigureAwait(false);

		    return new APIGatewayProxyResponse { StatusCode = 200 };
	    }
    }
}
