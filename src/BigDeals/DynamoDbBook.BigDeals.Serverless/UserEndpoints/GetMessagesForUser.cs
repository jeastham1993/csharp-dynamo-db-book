using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.UserEndpoints
{
	public class GetUser
	{
		private readonly IMessageRepository _messageRepository;

		public GetUser()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._messageRepository = serviceProvider.GetRequiredService<IMessageRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			var messages = await this._messageRepository.FindAllForUserAsync(
				               request.PathParameters["username"],
				               false).ConfigureAwait(false);

			if (messages != null)
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 200,
					Body = JsonConvert.SerializeObject(messages)
				};
			}
			else
			{
				return new APIGatewayProxyResponse
				{
					StatusCode = 404,
					Body = "Deal not found"
				};
			}
		}
	}
}