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
    public class AccountMigration
    {
	    private readonly AmazonDynamoDBClient _client;

	    public AccountMigration(
		    AmazonDynamoDBClient client)
	    {
		    this._client = client;
	    }

	    public async Task Execute()
	    {
		    var scanParams = new ScanRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    FilterExpression = "#type IN (:user, :org)",
			    ExpressionAttributeNames = new Dictionary<string, string>(1)
			    {
				    { "#type", "Type" }
			    },
			    ExpressionAttributeValues = new Dictionary<string, AttributeValue>(2)
			    {
				    { ":user", new AttributeValue("User") },
				    { ":org", new AttributeValue("Organization") },
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

		    if (item["Type"].S == "User")
		    {
			    var user = DynamoHelper.CreateFromItem<User>(item);
			    keys = user.AsKeys();
			    attributeValues.Add(
				    ":gsi1pk",
				    new AttributeValue(user.AsGsi1Pk()));
			    attributeValues.Add(
				    ":gsi1sk",
				    new AttributeValue(user.AsGsi1Sk()));
		    }
		    else
		    {
			    var org = DynamoHelper.CreateFromItem<Organization>(item);
			    keys = org.AsKeys();
			    attributeValues.Add(
				    ":gsi1pk",
				    new AttributeValue(org.AsGsi1Pk()));
			    attributeValues.Add(
				    ":gsi1sk",
				    new AttributeValue(org.AsGsi1Sk()));
		    }

		    var updateItemRequest = new UpdateItemRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    Key = keys,
			    UpdateExpression = "SET #gsi1pk = :gsi1pk, #gsi1sk = :gsi1sk",
			    ExpressionAttributeNames = new Dictionary<string, string>(2)
			    {
				    { "#gsi1pk", "GSI1PK" },
				    { "#gsi1sk", "GSI1SK" }
			    },
			    ExpressionAttributeValues = attributeValues
		    };

		    await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);
	    }
    }
}
