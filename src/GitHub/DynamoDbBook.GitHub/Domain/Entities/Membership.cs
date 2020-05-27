using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class Membership
    {
		public string OrganizationName { get; set; }

		public string Username { get; set; }

		public string Role { get; set; }

		public DateTime MemberSince { get; set; }
    }
}
