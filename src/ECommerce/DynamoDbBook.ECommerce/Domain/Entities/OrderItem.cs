using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public class OrderItem
    {
		public OrderItem()
		{
			this.ItemId = Ksuid.Generate().ToString();
		}
		public string OrderId { get; set; }

		public string ItemId { get; set; }

		public string Description { get; set; }

		public decimal Price { get; set; }

		public decimal Amount { get; set; }

		public decimal TotalCost => this.Price * this.Amount;
	}
}
