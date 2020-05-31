using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

namespace DynamoDbBook.BigDeals.Infrastructure.Models
{
    public static class BrandContainer
    {
		public static Dictionary<string, AttributeValue> AsKeys(int shard)
		{
			var keys = new Dictionary<string, AttributeValue>(2);
			keys.Add("PK", new AttributeValue($"BRANDS{shard}"));
			keys.Add("SK", new AttributeValue($"BRANDS{shard}"));

			return keys;
		}
    }
}
