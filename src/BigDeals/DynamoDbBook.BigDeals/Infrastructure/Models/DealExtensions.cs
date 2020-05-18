using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure.Models
{
    public static class DealExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(
			this Deal deal)
		{
			var keyAttributes = new Dictionary<string, AttributeValue>(2);
			keyAttributes.Add("PK", new AttributeValue($"DEAL#{deal.DealId}"));
			keyAttributes.Add("SK", new AttributeValue($"DEAL#{deal.DealId}"));

			return keyAttributes;
		}

		public static Dictionary<string, AttributeValue> AsGsiAttributes(this Deal deal)
		{
			if (deal == null)
			{
				throw new ArgumentNullException(nameof(deal));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("GSI1PK", new AttributeValue($"DEALS#{deal.CreatedAt.ToString("yyyy-MM-dd")} 00:00:00"));
			attributeMap.Add("GSI1SK", new AttributeValue($"DEAL#{deal.DealId}"));

			attributeMap.Add("GSI2PK", new AttributeValue($"BRAND#{deal.Brand}#{deal.CreatedAt.ToString("yyyy-MM-dd")} 00:00:00"));
			attributeMap.Add("GSI2SK", new AttributeValue($"DEAL#{deal.DealId}"));

			attributeMap.Add("GSI3PK", new AttributeValue($"CATEGORY#{deal.Category}#{deal.CreatedAt.ToString("yyyy-MM-dd")} 00:00:00"));
			attributeMap.Add("GSI3SK", new AttributeValue($"DEAL#{deal.DealId}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Deal deal)
		{
			if (deal == null)
			{
				throw new ArgumentNullException(nameof(deal));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in deal.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach (var map in deal.AsGsiAttributes())
			{
				attributeMap.Add(map.Key, map.Value);
			}

			attributeMap.Add("Type", new AttributeValue("Deal"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = deal.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Deal deal)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(deal));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
