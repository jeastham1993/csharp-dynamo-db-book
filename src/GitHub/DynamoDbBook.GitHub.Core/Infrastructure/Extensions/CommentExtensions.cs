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
    public static class CommentExtensions
    {
	    public static string GetPk(
		    this Comment comment)
	    {
		    return
			    $"{comment.GetType().Name.ToUpper()}#{comment.OwnerName.ToLower()}#{comment.RepoName.ToLower()}#{comment.PaddedTargetNumber}";
	    }

		public static Dictionary<string, AttributeValue> AsKeys(this Comment comment)
		{
			if (comment == null)
			{
				throw new ArgumentNullException(nameof(IssueComment));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add(
				"PK",
				new AttributeValue(
					comment.GetPk()));
			attributeMap.Add(
				"SK",
				new AttributeValue($"{comment.GetType().Name.ToUpper()}#{comment.Id}"));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Comment comment)
		{
			if (comment == null)
			{
				throw new ArgumentNullException(nameof(comment));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in comment.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue(comment.GetType().Name));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = comment.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Comment comment)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(comment));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}
    }
}
