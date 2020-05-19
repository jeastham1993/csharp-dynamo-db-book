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
	public static class CategoryExtensions
	{
		public static Dictionary<string, AttributeValue> AsKeys(
			this Category category)
		{
			var keyAttributes = new Dictionary<string, AttributeValue>(2);
			keyAttributes.Add("PK", new AttributeValue($"CATEGORY#{category.Name.ToUpper()}"));
			keyAttributes.Add("SK", new AttributeValue($"CATEGORY#{category.Name.ToUpper()}"));

			return keyAttributes;
		}

		public static Dictionary<string, AttributeValue> AsItem(
			this Category category)
		{
			if (category == null)
			{
				throw new ArgumentNullException(nameof(category));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in category.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("Category"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = category.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Category category)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(category));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
	}
}