using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public interface IUserRepository
	{
		Task<User> CreateAsync(
			User userToCreate);

		Task<IEnumerable<User>> GetAllUsersAsync();
	}
}
