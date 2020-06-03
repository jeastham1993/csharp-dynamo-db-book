using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure;
using DynamoDbBook.GitHub.Infrastructure.Extensions;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;
using DynamoDbBook.GitHubMigration.Core.Infrastructure.Extensions;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

namespace DynamoDbBook.GitHubMigration.Core.Infrastructure
{
    public class AppRepository : IAppRepository
    {
	    private readonly AmazonDynamoDBClient _client;

	    private readonly ILogger<AppRepository> _logger;

	    public AppRepository(
		    AmazonDynamoDBClient client,
		    ILogger<AppRepository> logger)
	    {
		    this._client = client;
		    this._logger = logger;
	    }

	    /// <inheritdoc />
	    public async Task<GitHubApp> CreateAppAsync(
		    GitHubApp app)
	    {
		    var putItemRequest = new PutItemRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    Item = app.AsItem(),
			    ConditionExpression = "attribute_not_exists(PK)"
		    };

		    try
		    {
			    await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

			    return app;
		    }
		    catch (ConditionalCheckFailedException ex)
		    {
			    this._logger.LogError("Repo with this name already exists for account.");

			    return null;
		    }
	    }

	    /// <inheritdoc />
	    public async Task<GitHubApp> InstallAppAsync(
		    GitHubAppInstallation appInstall)
	    {
		    var gitHubApp = new GitHubApp()
		    {
			    AppName = appInstall.AppName,
			    AppOwner = appInstall.AppOwner
		    };
		    var repo = new Repository()
		    {
			    OwnerName = appInstall.RepoOwner,
			    RepositoryName = appInstall.RepoName
		    };

		    var transactWrite = new TransactWriteItemsRequest()
		    {
			    TransactItems = new List<TransactWriteItem>(3)
			    {
				    new TransactWriteItem()
				    {
					    Put = new Put()
					    {
						    Item = appInstall.AsItem(),
						    TableName = DynamoDbConstants.TableName,
						    ConditionExpression = "attribute_not_exists(PK)"
					    }
				    },
				    new TransactWriteItem()
				    {
					    ConditionCheck = new ConditionCheck()
					    {
						    TableName = DynamoDbConstants.TableName,
						    ConditionExpression = "attribute_exists(PK)",
						    Key = gitHubApp.AsKeys()
					    }
				    },
				    new TransactWriteItem()
				    {
					    ConditionCheck = new ConditionCheck()
					    {
						    TableName = DynamoDbConstants.TableName,
						    ConditionExpression = "attribute_exists(PK)",
						    Key = repo.AsKeys()
					    }
				    }
			    }
		    };

		    var result = await this._client.TransactWriteItemsAsync(transactWrite).ConfigureAwait(false);

		    return gitHubApp;
	    }

	    /// <inheritdoc />
	    public async Task<IEnumerable<GitHubAppInstallation>> GetAppInstallations(
		    string appOwner,
		    string appName)
	    {
		    var attributeNames = new Dictionary<string, string>(2);
		    attributeNames.Add("#pk", "PK");

		    var app = new GitHubApp() { AppOwner = appOwner, AppName = appName};

		    var attributeValues = new Dictionary<string, AttributeValue>(3);
		    attributeValues.Add(
			    ":pk",
			    new AttributeValue(app.GetPk()));

		    var queryRequest = new QueryRequest()
		    {
			    TableName = DynamoDbConstants.TableName,
			    KeyConditionExpression = "#pk = :pk",
			    ExpressionAttributeNames = attributeNames,
			    ExpressionAttributeValues = attributeValues,
		    };

		    var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

		    var response = new List<GitHubAppInstallation>();

		    foreach (var item in queryResult.Items)
		    {
			    if (item["Type"].S == "GitHubAppInstallation")
			    {
				    response.Add(DynamoHelper.CreateFromItem<GitHubAppInstallation>(item));
			    }
		    }

		    return response;
	    }
    }
}
