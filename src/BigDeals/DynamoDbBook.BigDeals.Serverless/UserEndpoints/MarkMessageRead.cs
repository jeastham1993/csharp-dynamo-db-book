using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.UserEndpoints
{
	public class MarkMessageRead
	{
		private readonly IMessageRepository _messageRepository;

		public MarkMessageRead()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._messageRepository = serviceProvider.GetRequiredService<IMessageRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			await this._messageRepository.MarkMessageAsRead(
				HttpUtility.UrlDecode(request.PathParameters["username"]),
				HttpUtility.UrlDecode(request.PathParameters["messageId"])).ConfigureAwait(false);

			return new APIGatewayProxyResponse
			{
				StatusCode = 200
			};
		}
	}
}