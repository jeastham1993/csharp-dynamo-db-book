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

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoDbBook.ECommerce.Serverless.CustomerEndpoints
{
	public class CreateCustomer
	{
		private readonly ICustomerRepository _customerRepo;

		public CreateCustomer()
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

			var createCustomer = JsonConvert.DeserializeObject<CreateCustomerDTO>(request.Body);

			var newCustomer = Customer.Create(
				createCustomer.Username,
				createCustomer.Email,
				createCustomer.Name);

			createCustomer.Addresses.ForEach(
				a => newCustomer.AddAddress(
					a.Name,
					a.StreetAddress,
					a.PostalCode,
					a.Country));

			var createdCustomer = await this._customerRepo.CreateAsync(newCustomer).ConfigureAwait(false);

			if (createdCustomer != null)
			{
				return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(createdCustomer)};
			}
			else
			{
				return new APIGatewayProxyResponse { StatusCode = 400, Body = "Customer exists"};
			}
		}
	}
}