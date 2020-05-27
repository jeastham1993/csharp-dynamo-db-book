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
    public static class OrganizationExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this Organization organization)
		{
			if (organization == null)
			{
				throw new ArgumentNullException(nameof(organization));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add(
				"PK",
				new AttributeValue(
					$"ACCOUNT#{organization.Name.ToLower()}"));
			attributeMap.Add(
				"SK",
				new AttributeValue($"ACCOUNT#{organization.Name.ToLower()}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsGsiPk(
			this Organization organization)
		{
			return new Dictionary<string, AttributeValue>(1)
					   {
						   { "GSI3PK", new AttributeValue($"ACCOUNT#{organization.Name.ToLower()}") }
					   };
		}

		public static Dictionary<string, AttributeValue> AsGsi3(
			this Organization organization)
		{
			var attributes = organization.AsGsiPk();
			attributes.Add("GSI3SK", new AttributeValue($"ACCOUNT#{organization.Name.ToLower()}"));

			return attributes;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Organization organization)
		{
			if (organization == null)
			{
				throw new ArgumentNullException(nameof(organization));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach (var map in organization.AsGsi3())
			{
				attributeMap.Add(map.Key, map.Value);
			}

			foreach(var map in organization.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue(organization.GetType().Name));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = organization.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Organization organization)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(organization));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
