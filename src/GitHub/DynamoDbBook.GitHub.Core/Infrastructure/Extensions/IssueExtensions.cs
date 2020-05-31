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
    public static class IssueExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this Issue issue)
		{
			if (issue == null)
			{
				throw new ArgumentNullException(nameof(Issue));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"REPO#{issue.OwnerName}#{issue.RepositoryName}"));
			attributeMap.Add("SK", new AttributeValue($"ISSUE#{issue.PaddedIssueNumber}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Issue issue)
		{
			if (issue == null)
			{
				throw new ArgumentNullException(nameof(Issue));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in issue.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("Issue"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = issue.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Issue issue)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(issue));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
