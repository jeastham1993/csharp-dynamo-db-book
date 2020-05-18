using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public class Brand
    {
        public static Brand Create(string name, string logoUrl)
		{
			Guard.AgainstNullOrEmpty(
				name,
				nameof(name));

			Guard.AgainstNullOrEmpty(
				logoUrl,
				nameof(logoUrl));

			return new Brand() { BrandName = name, BrandLogoUrl = logoUrl };
		}
		public string BrandName { get; set; }

		public string BrandLogoUrl { get; set; }
    }
}
