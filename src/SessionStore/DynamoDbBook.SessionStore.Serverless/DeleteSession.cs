using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.SessionStore.Domain.Entities;
using DynamoDbBook.SessionStore.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using Guid = System.Guid;

namespace DynamoDbBook.SessionStore.Serverless
{
	public class DeleteSession
	{
		private readonly ISessionRepository _sessionRepository;

		public DeleteSession()
		{
			var serviceCollection = new ServiceCollection()
				.AddLogging()
				.ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._sessionRepository = serviceProvider.GetRequiredService<ISessionRepository>();
		}

		public async Task<APIGatewayProxyResponse> Execute(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			context.Logger.Log(request.Body);

			await this._sessionRepository.DeleteSessionForUser(request.PathParameters["username"])
				.ConfigureAwait(false);

			return new APIGatewayProxyResponse { StatusCode = 200};
		}
	}
}