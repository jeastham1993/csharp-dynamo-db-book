using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.GithUb.Serverless
{
    public static class ContainerSetup
    {
	    public static IServiceCollection ConfigureDynamoDb(
		    this IServiceCollection services)
	    {
		    var dynamoDbConfig = new AmazonDynamoDBConfig();
		    services.AddSingleton<AmazonDynamoDBClient>(new AmazonDynamoDBClient(dynamoDbConfig));

		    DynamoDbConstants.TableName = Environment.GetEnvironmentVariable("TABLE_NAME");

		    services.AddTransient<IUserRepository, UserRepository>();
		    services.AddTransient<IRepoRepository, RepoRepository>();
		    services.AddTransient<IInteractions, Interactions>();

		    return services;
	    }
    }
}
