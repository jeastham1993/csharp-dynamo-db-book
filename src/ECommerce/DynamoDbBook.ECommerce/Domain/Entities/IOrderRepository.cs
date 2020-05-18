using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public interface IOrderRepository
	{
		public Task<Order> GetOrderAsync(
			string username,
			string orderId);

		public Task<Order> CreateAsync(
			Order order);

		Task<IEnumerable<Order>> GetOrdersForCustomerAsync(
			string username);

		Task<Order> UpdateOrderStatusAsync(
			Order order,
			OrderStatus newStatus);
	}
}
