using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Serverless.CustomerEndpoints
{
	public class GetCustomer
	{
		private readonly ICustomerRepository _customerRepo;

		public GetCustomer()
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
			context.Logger.Log(request.Body);

			var customer = await this._customerRepo.GetCustomerAsync(request.PathParameters["username"]).ConfigureAwait(false);

			if (customer != null)
			{
				return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(customer)};
			}
			else
			{
				return new APIGatewayProxyResponse { StatusCode = 404, Body = "Customer not found"};
			}
		}
	}
}