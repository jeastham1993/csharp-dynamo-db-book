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
	public class CreateOrder
	{
		private readonly IOrderRepository _orderRepo;

		public CreateOrder()
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
			var order = JsonConvert.DeserializeObject<OrderDTO>(request.Body);

			var newOrder = Order.Create(
				order.Username,
				new Address(
					order.Address.Name,
					order.Address.StreetAddress,
					order.Address.PostalCode,
					order.Address.Country));

			order.OrderItems.ForEach(
				oi => newOrder.AddItem(
					oi.Description,
					oi.Amount,
					oi.Price));

			var createdOrder = await this._orderRepo.CreateAsync(newOrder).ConfigureAwait(false);

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