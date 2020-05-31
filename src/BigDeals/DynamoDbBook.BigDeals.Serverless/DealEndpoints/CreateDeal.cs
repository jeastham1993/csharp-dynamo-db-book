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
    public class CreateDeal
    {
	    private readonly IDealRepository _dealRepository;

	    public CreateDeal()
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
		    var dealDTO = JsonConvert.DeserializeObject<CreateDealDTO>(request.Body);

		    var deal = Deal.Create(dealDTO.Title);
		    deal.Price = dealDTO.Price;
		    deal.Brand = dealDTO.Brand;
		    deal.Category = dealDTO.Category;
		    deal.Link = dealDTO.Link;

		    var createdOrder = await this._dealRepository.CreateAsync(deal).ConfigureAwait(false);

		    if (createdOrder != null)
		    {
			    return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(createdOrder)};
		    }
		    else
		    {
			    return new APIGatewayProxyResponse { StatusCode = 400, Body = "Order exists"};
		    }
	    }
    }
}
