using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.SessionStore.Domain.Entities;
using DynamoDbBook.SessionStore.Infrastructure;
using DynamoDbBook.SessionStore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DynamoDbBook.SessionStore.Serverless
{
	public class CreateSessionHandler
	{
		private readonly ISessionRepository _sessionRepository;

		public CreateSessionHandler()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._sessionRepository = serviceProvider.GetRequiredService<ISessionRepository>();
		}

		public async Task<APIGatewayProxyResponse> CreateSession(
			APIGatewayProxyRequest request,
			ILambdaContext context)
		{
			context.Logger.Log(request.Body);

			var sessionRequest = JsonConvert.DeserializeObject<CreateSessionRequest>(request.Body);

			var response = await this._sessionRepository.CreateSession(Session.CreateNew(sessionRequest.Username))
							   .ConfigureAwait(false);

			return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(new CreateSessionResponse()
																										  {
																											  SessionId = response.SessionId,
																											  ExpiresAt = response.ExpiresAt,
																										  })};
		}
	}
}