using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.ViewModels
{
    public class AddressDTO
    {
		public string Name { get; set; }

		public string StreetAddress { get; set; }

		public string PostalCode { get; set; }

		public string Country { get; set; }
    }
}
