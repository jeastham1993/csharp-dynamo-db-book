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
    public static class GitHubAppExtensions
    {
	    public static string GetPk(
		    this GitHubApp gitHubApp)
	    {
		    return
			    $"APP#{gitHubApp.AppOwner.ToLower()}#{gitHubApp.AppName.ToLower()}";
	    }

	    public static Dictionary<string, AttributeValue> AsKeys(this GitHubApp gitHubApp)
	    {
		    if (gitHubApp == null)
		    {
			    throw new ArgumentNullException(nameof(gitHubApp));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    attributeMap.Add(
			    "PK",
			    new AttributeValue(
				    gitHubApp.GetPk()));
		    attributeMap.Add(
			    "SK",
			    new AttributeValue($"APP#{gitHubApp.AppOwner.ToLower()}#{gitHubApp.AppName.ToLower()}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsItem(this GitHubApp gitHubApp)
	    {
		    if (gitHubApp == null)
		    {
			    throw new ArgumentNullException(nameof(gitHubApp));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    foreach(var map in gitHubApp.AsKeys())
		    {
			    attributeMap.Add(map.Key, map.Value);	
		    }

		    attributeMap.Add("Type", new AttributeValue(gitHubApp.GetType().Name));
		    attributeMap.Add(
			    "Data",
			    new AttributeValue(){ M = gitHubApp.AsData()});

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsData(
		    this GitHubApp gitHubApp)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(gitHubApp));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }
    }
}
