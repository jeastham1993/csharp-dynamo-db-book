﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.ECommerce.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

		private readonly IOrderRepository _orderRepo;

		public OrderController(
			ILogger<OrderController> logger,
			IOrderRepository orderRepo)
		{
			_logger = logger;
			this._orderRepo = orderRepo;
		}
		

		[HttpGet("customers/{username}/orders/{orderId}")]
		public async Task<ActionResult<Order>> GetOrder(string username, string orderId)
		{
			var createdOrder = await this._orderRepo.GetOrderAsync(
								   username,
								   orderId).ConfigureAwait(false);

			if (createdOrder != null)
			{
				return this.Ok(createdOrder);
			}
			else
			{
				return this.BadRequest("Order exists");
			}
		}

		[HttpGet("customers/{username}/orders")]
		public async Task<ActionResult<IEnumerable<Order>>> GetOrdersForCustomer(string username)
		{
			var orders = await this._orderRepo.GetOrdersForCustomerAsync(
								   username).ConfigureAwait(false);

			if (orders != null)
			{
				return this.Ok(orders);
			}
			else
			{
				return this.BadRequest("Failure searching for orders");
			}
		}

		[HttpPost("customers/{username}/orders")]
		public async Task<ActionResult<Order>> CreateOrder(
			string username,
			[FromBody] OrderDTO order)
		{
			var newOrder = Order.Create(
				username,
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
				return this.Ok(createdOrder);
			}
			else
			{
				return this.BadRequest("Order exists");
			}
		}

		[HttpPost("customers/{username}/orders/{orderId}/status")]
		public async Task<ActionResult<Order>> UpdateOrderStatus(
			string username,
			string orderId,
			[FromBody] UpdateOrderStatusDTO updateOrderStatusReq)
		{
			return await this._orderRepo.UpdateOrderStatusAsync(
					   username,
					   orderId,
					   updateOrderStatusReq.NewStatus).ConfigureAwait(false);
		}
    }
}
