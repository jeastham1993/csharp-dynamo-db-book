using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Organization : Entity
    {
		public Organization()
			: base()
		{
		}
		public string Name { get; set; }

		public string OwnerName { get; set; }

		public int Members { get; set; }

		public PaymentPlan PaymentPlan { get; set; }
    }
}
