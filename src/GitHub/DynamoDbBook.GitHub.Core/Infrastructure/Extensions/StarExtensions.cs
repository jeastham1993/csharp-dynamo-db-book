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
    public static class StarExtensions
    {
	    public static Dictionary<string, AttributeValue> AsKeys(this Star star)
	    {
		    if (star == null)
		    {
			    throw new ArgumentNullException(nameof(star));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    attributeMap.Add(
			    "PK",
			    new AttributeValue(
				    $"REPO#{star.OwnerName.ToLower()}#{star.RepoName.ToLower()}"));
		    attributeMap.Add(
			    "SK",
			    new AttributeValue($"STAR#{star.Username.ToLower()}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsItem(this Star star)
	    {
		    if (star == null)
		    {
			    throw new ArgumentNullException(nameof(star));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    foreach(var map in star.AsKeys())
		    {
			    attributeMap.Add(map.Key, map.Value);	
		    }

		    attributeMap.Add("Type", new AttributeValue(star.GetType().Name));
		    attributeMap.Add(
			    "Data",
			    new AttributeValue(){ M = star.AsData()});

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsData(
		    this Star star)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(star));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }
    }
}
