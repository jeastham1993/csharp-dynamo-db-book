using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure.Extensions
{
    public static class MembershipExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this Membership membership)
		{
			if (membership == null)
			{
				throw new ArgumentNullException(nameof(membership));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add(
				"PK",
				new AttributeValue(
					$"ACCOUNT#{membership.OrganizationName.ToLower()}"));
			attributeMap.Add(
				"SK",
				new AttributeValue($"MEMBERSHIP#{membership.Username.ToLower()}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Membership membership)
		{
			if (membership == null)
			{
				throw new ArgumentNullException(nameof(membership));
			}

			var attributeMap = new Dictionary<string, AttributeValue>();

			foreach(var map in membership.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue(membership.GetType().Name));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = membership.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Membership membership)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(membership));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
