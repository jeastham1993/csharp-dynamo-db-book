using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2;

using DynamoDbBook.BigDeals.Core.Domain.Request;
using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.BigDeals.Serverless
{
    public static class ContainerSetup
    {
	    public static IServiceCollection ConfigureDynamoDb(
		    this IServiceCollection services)
	    {
		    var dynamoDbConfig = new AmazonDynamoDBConfig();
		    services.AddSingleton<AmazonDynamoDBClient>(new AmazonDynamoDBClient(dynamoDbConfig));

		    DynamoDbConstants.TableName = Environment.GetEnvironmentVariable("TABLE_NAME");

		    services.AddTransient<IBrandRepository, BrandRepository>();
		    services.AddTransient<IDealRepository, DealRepository>();
		    services.AddTransient<ICategoryRepository, CategoryRepository>();
		    services.AddTransient<IMessageRepository, MessageRepository>();
		    services.AddTransient<IUserRepository, UserRepository>();
		    services.AddSingleton<SendHotDealInteractor>();

		    return services;
	    }
    }
}
