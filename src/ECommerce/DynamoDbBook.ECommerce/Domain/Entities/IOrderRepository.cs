using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public interface IOrderRepository
	{
		public Task<Order> CreateAsync(
			Order order);
	}
}
