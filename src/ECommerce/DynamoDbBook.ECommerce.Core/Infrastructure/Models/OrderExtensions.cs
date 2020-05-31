using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Infrastructure.Models
{
    public static class OrderExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this Order order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"CUSTOMER#{order.Username}"));
			attributeMap.Add("SK", new AttributeValue($"#ORDER{order.OrderId}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsGsi1(this Order order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("GSI1PK", new AttributeValue($"ORDER#{order.OrderId}"));
			attributeMap.Add("GSI1SK", new AttributeValue($"ORDER#{order.OrderId}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Order order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in order.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach(var map in order.AsGsi1())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("Order"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = order.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Order order)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(order));
			var documentAttributeMap = document.ToAttributeMap();
			documentAttributeMap.Remove("items");
			documentAttributeMap.Remove("TotalAmount");
			documentAttributeMap.Remove("NumberItems");

			return documentAttributeMap;
		}

		public static Dictionary<string, AttributeValue> AsKeys(this OrderItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"ORDER#{item.OrderId}#ITEM#{item.ItemId}"));
			attributeMap.Add("SK", new AttributeValue($"ORDER#{item.OrderId}#ITEM#{item.ItemId}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsGsi1(this OrderItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("GSI1PK", new AttributeValue($"ORDER#{item.OrderId}"));
			attributeMap.Add("GSI1SK", new AttributeValue($"ITEM#{item.ItemId}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsAttributeMap(this OrderItem orderItem)
		{
			if (orderItem == null)
			{
				throw new ArgumentNullException(nameof(orderItem));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(7);

			foreach(var map in orderItem.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach(var map in orderItem.AsGsi1())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			var document = Document.FromJson(JsonConvert.SerializeObject(orderItem));
			document.ToAttributeMap();

			attributeMap.Add("Type", new AttributeValue("OrderItem"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = document.ToAttributeMap()});

			return attributeMap;
		}
    }
}
