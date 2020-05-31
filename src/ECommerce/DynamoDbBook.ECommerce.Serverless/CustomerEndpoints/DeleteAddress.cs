using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Serverless.CustomerEndpoints
{
	public class DeleteAddress
	{
		private readonly ICustomerRepository _customerRepo;

		public DeleteAddress()
		{
			var serviceCollection = new ServiceCollection()
				.AddLogging()
				.ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._customerRepo = serviceProvider.GetRequiredService<ICustomerRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			try
			{
				await this._customerRepo.DeleteAddressAsync(
					request.PathParameters["username"],
					request.PathParameters["name"]);

				return new APIGatewayProxyResponse { StatusCode = 200 };
			}
			catch (ConditionalCheckFailedException ex)
			{
				return new APIGatewayProxyResponse { StatusCode = 404, Body = "Customer not found"};
			}
		}
	}
}