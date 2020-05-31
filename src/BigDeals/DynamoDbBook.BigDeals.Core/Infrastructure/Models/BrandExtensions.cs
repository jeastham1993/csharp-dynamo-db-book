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
    public static class BrandExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(
			this Brand brand)
		{
			var keyAttributes = new Dictionary<string, AttributeValue>(2);
			keyAttributes.Add("PK", new AttributeValue($"BRAND#{brand.BrandName.ToUpper()}"));
			keyAttributes.Add("SK", new AttributeValue($"BRAND#{brand.BrandName.ToUpper()}"));

			return keyAttributes;
		}

		public static Dictionary<string, AttributeValue> AsItem(
			this Brand brand)
		{
			if (brand == null)
			{
				throw new ArgumentNullException(nameof(brand));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in brand.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("Brand"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = brand.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Brand brand)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(brand));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
