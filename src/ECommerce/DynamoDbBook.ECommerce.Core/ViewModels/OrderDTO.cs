using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.ViewModels
{
    public class OrderDTO
    {
		public string Username { get; set; }

		public AddressDTO Address { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }
    }
}
