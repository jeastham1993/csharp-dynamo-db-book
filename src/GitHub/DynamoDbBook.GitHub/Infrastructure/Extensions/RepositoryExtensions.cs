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
    public static class RepositoryExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this Repository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(Repository));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"REPO#{repository.OwnerName}#{repository.RepositoryName}"));
			attributeMap.Add("SK", new AttributeValue($"REPO#{repository.OwnerName}#{repository.RepositoryName}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Repository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(Repository));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in repository.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("Repository"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = repository.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Repository repository)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(repository));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
