using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public class User
    {
		public static User Create(
			string userName)
		{
			Guard.AgainstNullOrEmpty(
				userName,
				nameof(userName));

			return new User() { Username = userName, CreatedAt = DateTime.Now };
		}
		public string Username { get; set; }

		public DateTime CreatedAt { get; set; }
    }
}
