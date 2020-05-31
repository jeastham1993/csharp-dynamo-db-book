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
	public class UpdateOrderStatus
	{
		private readonly IOrderRepository _orderRepo;

		public UpdateOrderStatus()
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
			var updateOrderStatusReq = JsonConvert.DeserializeObject<UpdateOrderStatusDTO>(request.Body);

			var updatedOrder = await this._orderRepo.UpdateOrderStatusAsync(
				       request.PathParameters["username"],
				       request.PathParameters["orderId"],
				       updateOrderStatusReq.NewStatus).ConfigureAwait(false);

			if (updatedOrder != null)
			{
				return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(updatedOrder)};
			}
			else
			{
				return new APIGatewayProxyResponse { StatusCode = 404, Body = "Order not found"};
			}
		}
	}
}