using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure.Extensions
{
    public static class UserExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add(
				"PK",
				new AttributeValue(
					$"ACCOUNT#{user.Username.ToLower()}"));
			attributeMap.Add(
				"SK",
				new AttributeValue($"ACCOUNT#{user.Username.ToLower()}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsGsiPk(
			this User user)
		{
			return new Dictionary<string, AttributeValue>(1)
					   {
						   { "GSI3PK", new AttributeValue($"ACCOUNT#{user.Username.ToLower()}") }
					   };
		}

		public static Dictionary<string, AttributeValue> AsGsi3(
			this User user)
		{
			var attributes = user.AsGsiPk();
			attributes.Add("GSI3SK", new AttributeValue($"ACCOUNT#{user.Username.ToLower()}"));

			return attributes;
		}

		public static Dictionary<string, AttributeValue> AsItem(this User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach (var map in user.AsGsi3())
			{
				attributeMap.Add(map.Key, map.Value);
			}

			foreach(var map in user.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue(user.GetType().Name));
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
