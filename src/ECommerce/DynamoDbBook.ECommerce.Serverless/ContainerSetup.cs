using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

using Amazon.DynamoDBv2;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.ECommerce.Serverless
{
    public static class ContainerSetup
    {
	    public static IServiceCollection ConfigureDynamoDb(
		    this IServiceCollection services)
	    {
		    var dynamoDbConfig = new AmazonDynamoDBConfig();
		    services.AddSingleton<AmazonDynamoDBClient>(new AmazonDynamoDBClient(dynamoDbConfig));

		    DynamoDbConstants.TableName = Environment.GetEnvironmentVariable("TABLE_NAME");

		    services.AddTransient<ICustomerRepository, CustomerRepository>();
		    services.AddTransient<IOrderRepository, OrderRepository>();

		    return services;
	    }
    }
}
