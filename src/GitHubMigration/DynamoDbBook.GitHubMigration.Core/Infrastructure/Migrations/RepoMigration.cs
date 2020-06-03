using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure;
using DynamoDbBook.GitHub.Infrastructure.Extensions;
using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.GitHubMigration.Core.Infrastructure.Migrations
{
    public class RepoMigration
    {
	    private readonly AmazonDynamoDBClient _client;

	    public RepoMigration(
		    AmazonDynamoDBClient client)
	    {
		    this._client = client;
	    }

	    public async Task Execute()
	    {
		    var scanParams = new ScanRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    FilterExpression = "#type IN (:repo, :issue, :pr)",
			    ExpressionAttributeNames = new Dictionary<string, string>(1)
			    {
				    { "#type", "Type" }
			    },
			    ExpressionAttributeValues = new Dictionary<string, AttributeValue>(2)
			    {
				    { ":repo", new AttributeValue("Repository") },
				    { ":issue", new AttributeValue("Issue") },
				    { ":pr", new AttributeValue("PullRequest") },
			    }
		    };

		    Dictionary<string, AttributeValue> exclusiveStartKey = null;

		    var resultCount = 0;

		    while (true)
		    {
				if (exclusiveStartKey != null)
				{
					scanParams.ExclusiveStartKey = exclusiveStartKey;
				}

				var results = await this._client.ScanAsync(scanParams).ConfigureAwait(false);

			    if (results.Items.Any())
				{
					foreach (var item in results.Items)
					{
						resultCount++;

						await this.UpdateItem(item).ConfigureAwait(false);
					}
				}

			    if (results.LastEvaluatedKey == null || results.LastEvaluatedKey.Any() == false)
			    {
				    break;   
			    }
			    else
			    {
				    exclusiveStartKey = results.LastEvaluatedKey;
			    }
		    }
	    }

	    private async Task UpdateItem(
		    Dictionary<string, AttributeValue> item)
	    {
		    var keys = new Dictionary<string, AttributeValue>();
			var attributeValues = new Dictionary<string, AttributeValue>();
			var updateExpression = string.Empty;
			var attributeNames = new Dictionary<string, string>();

		    if (item["Type"].S == "Repository")
		    {
			    updateExpression = "SET #gsi4pk = :gsi4pk, #gsi4sk = :gsi4sk, #gsi5pk = :gsi5pk, #gsi5sk = :gsi5sk";
			    attributeNames = new Dictionary<string, string>(2)
			    {
				    { "#gsi4pk", "GSI4PK" },
				    { "#gsi4sk", "GSI4SK" },
				    { "#gsi5pk", "GSI5PK" },
				    { "#gsi5sk", "GSI5SK" },
			    };

			    var repo = DynamoHelper.CreateFromItem<Repository>(item);
			    keys = repo.AsKeys();
			    attributeValues.Add(
				    ":gsi4pk",
				    new AttributeValue(repo.GetGsi4Pk()));
			    attributeValues.Add(
				    ":gsi4sk",
				    new AttributeValue(repo.GetGsi4Sk()));
			    attributeValues.Add(
				    ":gsi5pk",
				    new AttributeValue(repo.GetGsi5Pk()));
			    attributeValues.Add(
				    ":gsi5sk",
				    new AttributeValue(repo.GetGsi5Sk()));
		    }
		    else if (item["Type"].S == "Issue")
		    {
			    updateExpression = "SET #gsi4pk = :gsi4pk, #gsi4sk = :gsi4sk";
			    attributeNames = new Dictionary<string, string>(2)
			    {
				    { "#gsi4pk", "GSI4PK" },
				    { "#gsi4sk", "GSI4SK" },
			    };
			    var issue = DynamoHelper.CreateFromItem<Issue>(item);
			    keys = issue.AsKeys();
			    attributeValues.Add(
				    ":gsi4pk",
				    new AttributeValue(issue.GetGsi4Pk()));
			    attributeValues.Add(
				    ":gsi4sk",
				    new AttributeValue(issue.GetGsi4Sk()));
		    }
		    else
		    {
			    updateExpression = "SET #gsi5pk = :gsi5pk, #gsi5sk = :gsi5sk";
			    attributeNames = new Dictionary<string, string>(2)
			    {
				    { "#gsi5pk", "GSI5PK" },
				    { "#gsi5sk", "GSI5SK" },
			    };
			    var pr = DynamoHelper.CreateFromItem<PullRequest>(item);
			    keys = pr.AsKeys();
			    attributeValues.Add(
				    ":gsi5pk",
				    new AttributeValue(pr.GetGsi5Pk()));
			    attributeValues.Add(
				    ":gsi5sk",
				    new AttributeValue(pr.GetGsi5Sk()));
		    }

		    var updateItemRequest = new UpdateItemRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    Key = keys,
			    UpdateExpression = updateExpression,
			    ExpressionAttributeNames = attributeNames,
			    ExpressionAttributeValues = attributeValues
		    };

		    await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);
	    }
    }
}
