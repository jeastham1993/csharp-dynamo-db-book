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
    public static class PullRequestExtensions
    {
		public static Dictionary<string, AttributeValue> AsKeys(this PullRequest pullRequest)
		{
			if (pullRequest == null)
			{
				throw new ArgumentNullException(nameof(PullRequest));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"PR#{pullRequest.OwnerName.ToLower()}#{pullRequest.RepoName.ToLower()}#{pullRequest.PaddedPullRequestNumber}"));
			attributeMap.Add("SK", new AttributeValue($"PR#{pullRequest.OwnerName.ToLower()}#{pullRequest.RepoName.ToLower()}#{pullRequest.PaddedPullRequestNumber}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsGsi1(
			this PullRequest pullRequest)
		{
			return new Dictionary<string, AttributeValue>(2)
					   {
						   {
							   "GSI1PK",
							   new AttributeValue(
								   $"REPO#{pullRequest.OwnerName.ToLower()}#{pullRequest.RepoName.ToLower()}")
						   },
						   { "GSI1SK", new AttributeValue($"PR#{pullRequest.PaddedPullRequestNumber}") }
					   };
		}

		public static Dictionary<string, AttributeValue> AsItem(this PullRequest pullRequest)
		{
			if (pullRequest == null)
			{
				throw new ArgumentNullException(nameof(PullRequest));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in pullRequest.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach (var gsiMap in pullRequest.AsGsi1())
			{
				attributeMap.Add(
					gsiMap.Key,
					gsiMap.Value);
			}

			attributeMap.Add("Type", new AttributeValue("PullRequest"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = pullRequest.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this PullRequest pullRequest)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(pullRequest));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}

		public static Dictionary<string, AttributeValue> GetGsis(this PullRequest pr)
		{
			return new Dictionary<string, AttributeValue>(2)
			{
				{ "GSI5PK", new AttributeValue(pr.GetGsi5Pk()) },
				{ "GSI5SK", new AttributeValue(pr.GetGsi5Sk()) }
			};
		}

		public static string GetGsi5Pk(this PullRequest pr)
		{
			return $"REPO#{pr.OwnerName.ToLower()}#{pr.RepoName.ToLower()}";
		}

		public static string GetGsi5Sk(this PullRequest pr)
		{
			if (pr.Status == "Open")
			{
				return $"PR#OPEN#{pr.PaddedPullRequestDifference}";
			}
			
			return $"#PR#CLOSED#{pr.PaddedPullRequestNumber}";
		}
    }
}
