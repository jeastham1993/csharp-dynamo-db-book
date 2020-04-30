using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.ViewModels
{
    public class OrderItemDTO
    {
		public string Description { get; set; }

		public decimal Price { get; set; }

		public decimal Amount { get; set; }
    }
}
