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
    public static class GitHubAppInstallExtensions
    {
	    public static string GetPk(
		    this GitHubAppInstallation gitHubApp)
	    {
		    return
			    $"APP#{gitHubApp.AppOwner.ToLower()}#{gitHubApp.AppName.ToLower()}";
	    }

	    public static Dictionary<string, AttributeValue> AsKeys(this GitHubAppInstallation gitHubApp)
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
			    new AttributeValue($"REPO#{gitHubApp.RepoOwner.ToLower()}#{gitHubApp.RepoName.ToLower()}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsGsi(this GitHubAppInstallation gitHubApp)
	    {
		    if (gitHubApp == null)
		    {
			    throw new ArgumentNullException(nameof(gitHubApp));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    attributeMap.Add(
			    "GSI1PK",
			    new AttributeValue(
				    $"REPO#{gitHubApp.RepoOwner.ToLower()}#{gitHubApp.RepoName.ToLower()}"));
		    attributeMap.Add(
			    "GSI1SK",
			    new AttributeValue($"REPOAPP#{gitHubApp.AppOwner.ToLower()}#${gitHubApp.AppName.ToLower()}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsItem(this GitHubAppInstallation gitHubApp)
	    {
		    if (gitHubApp == null)
		    {
			    throw new ArgumentNullException(nameof(gitHubApp));
		    }

		    var attributeMap = gitHubApp.AsKeys();

		    foreach(var map in gitHubApp.AsGsi())
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
		    this GitHubAppInstallation gitHubApp)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(gitHubApp));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }
    }
}
