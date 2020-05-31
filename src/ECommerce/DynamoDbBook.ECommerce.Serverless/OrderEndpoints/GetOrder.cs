using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Serverless.OrderEndpoints
{
	public class GetOrder
	{
		private readonly IOrderRepository _orderRepo;

		public GetOrder()
		{
			var serviceCollection = new ServiceCollection()
				.AddLogging()
				.ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			context.Logger.Log($"{request.PathParameters["username"]} : {request.PathParameters["orderId"]}");
			var order = await this._orderRepo.GetOrderAsync(
				                   request.PathParameters["username"],
				                   request.PathParameters["orderId"]).ConfigureAwait(false);

			if (order != null)
			{
				return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(order)};
			}
			else
			{
				return new APIGatewayProxyResponse { StatusCode = 404, Body = "Order not found"};
			}
		}
	}
}