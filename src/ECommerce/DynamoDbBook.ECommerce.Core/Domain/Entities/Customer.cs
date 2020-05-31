using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.ECommerce.ViewModels;
using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public class Customer
    {
		[JsonProperty("addresses")]
		private List<Address> _addresses;

		[JsonConstructor]
		private Customer()
		{
			this._addresses = new List<Address>();
		}

		[JsonProperty]
		public string Username { get; private set; }

		[JsonProperty]
		public string Email { get; private set; }

		[JsonProperty]
		public string Name { get; set; }

		[JsonIgnore]
		public IReadOnlyCollection<Address> Addresses => this._addresses;

		public static Customer Create(
			string username,
			string email,
			string name)
		{
			Guard.AgainstNullOrEmpty(
				username,
				nameof(username));
			Guard.AgainstNullOrEmpty(
				email,
				nameof(email));

			return new Customer() { Username = username, Email = email, Name = name };
		}

		public Address AddAddress(
			string name,
			string streetAddress,
			string postalCode,
			string country)
		{
			var newAddress = new Address(
				name,
				streetAddress,
				postalCode,
				country);

			this._addresses.Add(newAddress);

			return newAddress;
		}
	}
}
