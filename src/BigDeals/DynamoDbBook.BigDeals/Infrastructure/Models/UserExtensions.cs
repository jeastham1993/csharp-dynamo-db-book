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
    public static class UserExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(
			this User user)
		{
			var keyAttributes = new Dictionary<string, AttributeValue>(2);
			keyAttributes.Add("PK", new AttributeValue($"USER#{user.Username}"));
			keyAttributes.Add("SK", new AttributeValue($"USER#{user.Username}"));

			return keyAttributes;
		}

		public static Dictionary<string, AttributeValue> AsItem(this User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in user.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("User"));
			attributeMap.Add(
				"UserIndex",
				new AttributeValue($"USER#{user.Username}"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = user.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this User user)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(user));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
