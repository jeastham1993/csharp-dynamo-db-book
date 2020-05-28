using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities.Reactions;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure.Extensions
{
    public static class ReactionExtensions
    {
	    public static Dictionary<string, AttributeValue> AsKeys(this Reaction reaction)
	    {
		    if (reaction == null)
		    {
			    throw new ArgumentNullException(nameof(IssueReaction));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    attributeMap.Add(
			    "PK",
			    new AttributeValue(
				    $"{reaction.GetType().Name.ToUpper()}REACTION#{reaction.OwnerName.ToLower()}#{reaction.RepoName.ToLower()}#{reaction.Identifier}#{reaction.ReactingUsername}"));
		    attributeMap.Add(
			    "SK",
			    new AttributeValue(
				    $"{reaction.GetType().Name.ToUpper()}REACTION#{reaction.OwnerName.ToLower()}#{reaction.RepoName.ToLower()}#{reaction.Identifier}#{reaction.ReactingUsername}"));

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsItem(this Reaction reaction)
	    {
		    if (reaction == null)
		    {
			    throw new ArgumentNullException(nameof(reaction));
		    }

		    var attributeMap = new Dictionary<string, AttributeValue>(5);

		    foreach(var map in reaction.AsKeys())
		    {
			    attributeMap.Add(map.Key, map.Value);	
		    }

		    attributeMap.Add("Type", new AttributeValue(reaction.GetType().Name));
		    attributeMap.Add(
			    "Data",
			    new AttributeValue(){ M = reaction.AsData()});

		    return attributeMap;
	    }

	    public static Dictionary<string, AttributeValue> AsData(
		    this Reaction reaction)
	    {
		    var document = Document.FromJson(JsonConvert.SerializeObject(reaction));
		    var documentAttributeMap = document.ToAttributeMap();

		    return documentAttributeMap;
	    }
    }
}
