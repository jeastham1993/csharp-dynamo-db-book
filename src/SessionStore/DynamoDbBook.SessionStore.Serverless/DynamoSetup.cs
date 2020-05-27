using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

using Amazon.DynamoDBv2;

using DynamoDbBook.SessionStore.Domain.Entities;
using DynamoDbBook.SessionStore.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.SessionStore.Serverless
{
    public static class DynamoSetup
    {
		public static IServiceCollection ConfigureDynamoDb(
			this IServiceCollection services)
		{
			var dynamoDbConfig = new AmazonDynamoDBConfig();
			services.AddSingleton<AmazonDynamoDBClient>(new AmazonDynamoDBClient(dynamoDbConfig));

			services.AddTransient<ISessionRepository, SessionRepository>();

			DynamoDbConstants.TableName = Environment.GetEnvironmentVariable("TABLE_NAME");

			return services;
		}
    }
}
