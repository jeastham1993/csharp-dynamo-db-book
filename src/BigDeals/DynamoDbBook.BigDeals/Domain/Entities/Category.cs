using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public class Category
    {
		public static Category Create(string name, List<Deal> featuredDeals)
		{
			Guard.AgainstNullOrEmpty(
				name,
				nameof(name));

			return new Category() { Name = name, FeaturedDeals = featuredDeals, WatchCount = 0, LikesCount = 0};
		}

        public string Name { get; set; }

        public List<Deal> FeaturedDeals { get; set; }

        public int WatchCount { get; set; }

        public int LikesCount { get; set; }
    }
}
