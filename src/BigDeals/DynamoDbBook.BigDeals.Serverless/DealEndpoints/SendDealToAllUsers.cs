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
    public class SendDealToAllUsers
    {
	    private readonly IDealRepository _dealRepository;

	    public SendDealToAllUsers()
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
		    var deal = await this._dealRepository.(request.PathParameters["dealId"]).ConfigureAwait(false);

		    if (deal != null)
		    {
			    return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(deal)};
		    }
		    else
		    {
			    return new APIGatewayProxyResponse { StatusCode = 404, Body = "Deal not found"};
		    }
	    }
    }
}
