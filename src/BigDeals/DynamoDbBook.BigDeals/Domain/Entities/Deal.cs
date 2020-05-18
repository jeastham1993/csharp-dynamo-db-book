using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public class Deal
    {
		[JsonProperty]
		public string DealId { get; private set; }

		[JsonProperty]
		public DateTime CreatedAt { get; private set; }

		public string Title { get; set; }

		public string Link { get; set; }

		public decimal Price { get; set; }

		public string Category { get; set; }

		public string Brand { get; set; }

		public static Deal Create(string title)
		{
			Guard.AgainstNullOrEmpty(
				title,
				nameof(title));

			return new Deal()
					   {
						   DealId = Ksuid.Generate().ToString().Replace("/", string.Empty),
						   CreatedAt = DateTime.Now,
						   Title = title,
					   };
		}
    }
}
