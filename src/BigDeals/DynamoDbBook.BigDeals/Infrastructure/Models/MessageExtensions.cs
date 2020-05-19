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
    public static class MessageExtensions
    {
        public static Dictionary<string, AttributeValue> AsKeys(
			this Message message)
		{
			var keyAttributes = new Dictionary<string, AttributeValue>(2);
			keyAttributes.Add("PK", new AttributeValue($"MESSAGE#{message.Username}"));
			keyAttributes.Add("SK", new AttributeValue($"MESSAGE#{message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")}"));

			return keyAttributes;
		}

		public static Dictionary<string, AttributeValue> AsGsiAttributes(this Message message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(2);

			if (message.Unread)
			{
				attributeMap.Add("GSI1PK", new AttributeValue($"MESSAGE#{message.Username}"));
				attributeMap.Add("GSI1SK", new AttributeValue($"MESSAGE#{message.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")}"));
			}

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Message message)
		{
			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in message.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach (var map in message.AsGsiAttributes())
			{
				attributeMap.Add(map.Key, map.Value);
			}

			var messageData = message.AsData();
			messageData.Remove("Unread");

			attributeMap.Add("Type", new AttributeValue("Message"));
			attributeMap.Add("Unread", new AttributeValue() { BOOL = message.Unread});
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = messageData});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Message message)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(message));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
