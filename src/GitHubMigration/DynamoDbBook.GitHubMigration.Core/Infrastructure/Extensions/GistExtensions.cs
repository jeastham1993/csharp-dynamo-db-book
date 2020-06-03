using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHubMigration.Core.Infrastructure.Extensions
{
    public static class GistExtensions
    {
	    public static string GetPk(
		    this Gist gist)
	    {
		    return
			    $"ACCOUNT#{gist.OwnerName?.ToLower()}";
	    }

	    public static Dictionary<string, AttributeValue> AsKeys(this Gist gist)
	    {
		    if (gist == null)
		    {
			    throw new ArgumentNullException(nameof(gist));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    attributeMap.Add(
			    "PK",
			    new AttributeValue(
				    gist.GetPk()));
		    attributeMap.Add(
			    "SK",
			    new AttributeValue($"#GIST#{gist.GistId}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsItem(this Gist gist)
	    {
		    if (gist == null)
		    {
			    throw new ArgumentNullException(nameof(gist));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    foreach(var map in gist.AsKeys())
		    {
			    attributeMap.Add(map.Key, map.Value);	
		    }

		    attributeMap.Add("Type", new AttributeValue(gist.GetType().Name));
		    attributeMap.Add(
			    "Data",
			    new AttributeValue(){ M = gist.AsData()});

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsData(
		    this Gist gist)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(gist));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }
    }
}
