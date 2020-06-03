using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;
using DynamoDbBook.GitHubMigration.Core.Infrastructure.Migrations;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.Migrations
{
    public class RepoMigrationHandler
    {
	    private readonly RepoMigration _migrator;

	    public RepoMigrationHandler()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._migrator = serviceProvider.GetRequiredService<RepoMigration>();
	    }

	    public async Task Execute(
		    ILambdaContext context)
	    {
		    await this._migrator.Execute().ConfigureAwait(false);
	    }
    }
}
