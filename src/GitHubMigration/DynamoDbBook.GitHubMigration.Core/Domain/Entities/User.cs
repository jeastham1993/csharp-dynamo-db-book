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
			this.Organizations = new Dictionary<string, string>(0);
		}

		public string Username { get; set; }

		public string Name { get; set; }

		public Dictionary<string, string> Organizations { get; set; }

		public PaymentPlan PaymentPlan { get; set; }
    }
}
