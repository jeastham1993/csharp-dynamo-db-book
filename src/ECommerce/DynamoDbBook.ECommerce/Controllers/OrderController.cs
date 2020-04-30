using System.Collections.Generic;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.ECommerce.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

		[HttpPost]
		public async Task<ActionResult<Customer>> CreateCustomer(
			[FromBody] OrderDTO order)
		{
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
				return this.Ok(createdOrder);
			}
			else
			{
				return this.BadRequest("Customer exists");
			}
		}
    }
}
