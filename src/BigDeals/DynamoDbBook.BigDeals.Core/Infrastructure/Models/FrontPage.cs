using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure.Models
{
    public class FrontPage
    {
		public static Dictionary<string, AttributeValue> AsKeys(int shard)
		{
			var itemData = new Dictionary<string, AttributeValue>(2);
			itemData.Add("PK", new AttributeValue($"FRONTPAGE{shard}"));
			itemData.Add("SK", new AttributeValue($"FRONTPAGE{shard}"));

			return itemData;
		}

		public static Dictionary<string, AttributeValue> AsItem(
			int shard,
			IEnumerable<Deal> deals)
		{
			var itemData = FrontPage.AsKeys(shard);
			
			itemData.Add("Type", new AttributeValue("FrontPage"));
			itemData.Add(
				"Data",
				new AttributeValue(JsonConvert.SerializeObject(deals)));

			return itemData;
		}
    }
}
