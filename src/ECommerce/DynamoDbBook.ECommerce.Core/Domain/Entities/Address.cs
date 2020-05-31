using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public class Address
    {
		[JsonConstructor]
		private Address()
		{
		}

		public Address(
			string name,
			string streetAddress,
			string postalCode,
			string country)
		{
			Guard.AgainstNullOrEmpty(name, nameof(name));

			this.Name = name;
			this.StreetAddress = streetAddress;
			this.PostalCode = postalCode;
			this.Country = country;
		}

		[JsonProperty]
		public string Name { get; private set; }

		[JsonProperty]
		public string StreetAddress { get; private set; }

		[JsonProperty]
		public string PostalCode { get; private set; }

		[JsonProperty]
		public string Country { get; private set; }
    }
}
