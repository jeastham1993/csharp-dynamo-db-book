using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			attributeMap.Add("SK", new AttributeValue($"#ORDER'{order.OrderId}"));

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

		public static Dictionary<string, AttributeValue> AsAttributeMap(this Order order)
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

			var addressData = new Dictionary<string, AttributeValue>();

			attributeMap.Add("Type", new AttributeValue("Order"));
			attributeMap.Add("Username", new AttributeValue(order.Username));
			attributeMap.Add("OrderId", new AttributeValue(order.OrderId));
			attributeMap.Add("Address", new AttributeValue(JsonConvert.SerializeObject(order.Address)));
			attributeMap.Add("CreatedAt", new AttributeValue(order.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")));
			attributeMap.Add("Status", new AttributeValue(order.Status.ToString()));
			attributeMap.Add("TotalAmount", new AttributeValue()
												{
													N = order.TotalAmount.ToString("n2")
												});
			attributeMap.Add("NumberItems", new AttributeValue()
												{
													N = order.NumberItems.ToString("n0")
												});

			return attributeMap;
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

		public static Dictionary<string, AttributeValue> AsAttributeMap(this OrderItem order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(7);

			foreach(var map in order.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach(var map in order.AsGsi1())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			var addressData = new Dictionary<string, AttributeValue>();

			attributeMap.Add("Type", new AttributeValue("OrderItem"));
			attributeMap.Add("OrderId", new AttributeValue(order.OrderId));
			attributeMap.Add("ItemId", new AttributeValue(order.ItemId));
			attributeMap.Add("Description", new AttributeValue(order.Description));
			attributeMap.Add("Price", new AttributeValue()
										  {
											  N = order.Price.ToString()
										  });
			attributeMap.Add("Amount", new AttributeValue()
										  {
											  N = order.Amount.ToString()
										  });
			attributeMap.Add("TotalCost", new AttributeValue()
										  {
											  N = order.TotalCost.ToString()
										  });

			return attributeMap;
		}
    }
}
