using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

namespace DynamoDbBook.BigDeals.Infrastructure.Models
{
    public static class BrandLike
    {
		public static Dictionary<string, AttributeValue> GenerateKeys(string brand, string username)
		{
			var keys = new Dictionary<string, AttributeValue>(2);
			keys.Add("PK", new AttributeValue($"BRANDLIKE#{brand}#{username}"));
			keys.Add("SK", new AttributeValue($"BRANDLIKE#{brand}#{username}"));

			return keys;
		}

		public static Dictionary<string, string> GenerateExpressionAttributeNames()
		{
			var attributeNames = new Dictionary<string, string>(1);
			attributeNames.Add("#data", "Data");
			attributeNames.Add("#likes", "LikesCount");

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
