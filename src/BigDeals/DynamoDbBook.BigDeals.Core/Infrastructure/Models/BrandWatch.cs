using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

namespace DynamoDbBook.BigDeals.Infrastructure.Models
{
    public class BrandWatch
    {
		public static Dictionary<string, AttributeValue> GenerateKeys(string brand, string username)
		{
			var keys = new Dictionary<string, AttributeValue>(2);
			keys.Add("PK", new AttributeValue($"BRANDWATCH#{brand}"));
			keys.Add("SK", new AttributeValue($"USER#{username}"));

			return keys;
		}

		public static Dictionary<string, string> GenerateExpressionAttributeNames()
		{
			var attributeNames = new Dictionary<string, string>(1);
			attributeNames.Add("#data", "Data");
			attributeNames.Add("#watcher", "WatchCount");

			return attributeNames;
		}

		public static Dictionary<string, AttributeValue> GenerateExpressionAttributeValues()
		{
			var attributeNames = new Dictionary<string, AttributeValue>(1);
			attributeNames.Add(
				":incr",
				new AttributeValue() { N = "1" });

			return attributeNames;
		}
	}
}
