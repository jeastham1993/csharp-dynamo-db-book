using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class User : Entity
    {
		public User()
			: base()
		{
		}

		public string Username { get; set; }

		public string Name { get; set; }

		public IEnumerable<Organization> Organizations { get; set; }

		public PaymentPlan PaymentPlan { get; set; }
    }
}
