using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.ECommerce.ViewModels;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public interface ICustomerRepository
	{
		Task<Customer> CreateAsync(
			Customer customerToCreate);

		
		Task AddAddressAsync(
			string username,
			Address address);
	}
}
