using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.ViewModels
{
    public class CreateCustomerDTO
    {
		public string Name { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

        public List<AddressDTO> Addresses { get; set; }
    }
}
