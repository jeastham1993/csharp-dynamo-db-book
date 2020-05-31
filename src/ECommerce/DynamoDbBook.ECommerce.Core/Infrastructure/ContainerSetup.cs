using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;

using DynamoDbBook.ECommerce.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.ECommerce.Infrastructure
{
    public static class ContainerSetup
    {
		public static IServiceCollection AddDynamoDb(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			var dynamoDbConfig = new AmazonDynamoDBConfig();

			if (configuration["Debug"] == "True")
			{
				dynamoDbConfig.ServiceURL = "http://localhost:4566";

				var client = new AmazonDynamoDBClient(dynamoDbConfig);

				services.AddSingleton<AmazonDynamoDBClient>(client);
			}
			else
			{
				dynamoDbConfig.RegionEndpoint = RegionEndpoint.EUWest2;

				var client = new AmazonDynamoDBClient(
					new BasicAWSCredentials(
						configuration["DynamoDb:AccessKey"],
						configuration["DynamoDb:SecretKey"]),
					dynamoDbConfig);

				services.AddSingleton<AmazonDynamoDBClient>(client);
			}

			services.AddTransient<ICustomerRepository, CustomerRepository>();
			services.AddTransient<IOrderRepository, OrderRepository>();

			return services;
		}
    }
}
