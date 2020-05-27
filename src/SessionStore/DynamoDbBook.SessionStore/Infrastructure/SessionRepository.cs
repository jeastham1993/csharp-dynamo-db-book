using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.SessionStore.Domain.Entities;
using DynamoDbBook.SessionStore.Domain.Exceptions;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Amazon.Runtime.Internal.Util.ILogger;

namespace DynamoDbBook.SessionStore.Infrastructure
{
    public class SessionRepository : ISessionRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<SessionRepository> _logger;

		public SessionRepository(
			AmazonDynamoDBClient client,
			ILogger<SessionRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Session> CreateSession(
			Session sessionToCreate)
		{
			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = sessionToCreate.AssAttributeMap(),
										 ConditionExpression = "attribute_not_exists(SessionId)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return sessionToCreate;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Holy moley -- a UUID collision!");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Session> GetSession(
			Guid token)
		{
			var query = new QueryRequest(DynamoDbConstants.TableName)
							{
								KeyConditionExpression = "#SessionId = :SessionId",
								ExpressionAttributeNames = new Dictionary<string, string>(1)
															   {
																   { "#SessionId", "SessionId" },
															   },
								ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																{
																	{ ":SessionId", new AttributeValue(token.ToString()) },
																},
							};

			var result = await this._client.QueryAsync(query).ConfigureAwait(false);

			if (result.Items.Any() == false)
			{
				throw new InvalidSessionException($"Session {token} not found");
			}

			var document = Document.FromAttributeMap(result.Items[0]);

			return JsonConvert.DeserializeObject<Session>(document.ToJson());
		}

		/// <inheritdoc />
		public async Task DeleteSessionForUser(
			string username)
		{
			var query = new QueryRequest(DynamoDbConstants.TableName)
							{
								KeyConditionExpression = "#username = :username",
								IndexName = "UserIndex",
								ExpressionAttributeNames = new Dictionary<string, string>(1)
															   {
																   { "#username", "Username" },
															   },
								ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																{
																	{ ":username", new AttributeValue(username) },
																},
							};

			var result = await this._client.QueryAsync(query).ConfigureAwait(false);

			if (result.Items.Any())
			{
				var deleteRequest = new List<WriteRequest>();


				foreach (var resultItem in result.Items)
				{
					var writeRequest = new WriteRequest
										   {
											   DeleteRequest = new DeleteRequest
																   {
																	   Key =
																		   new Dictionary<string, AttributeValue>()
																			   {
																				   {
																					   "SessionId",
																					   resultItem["SessionId"]
																				   }
																			   }
																   }
										   };
					deleteRequest.Add(writeRequest);
				}

				var request = new BatchWriteItemRequest
								  {
									  RequestItems = new Dictionary<string, List<WriteRequest>>
														 {
															 { DynamoDbConstants.TableName, deleteRequest }
														 }
								  };

				await this._client.BatchWriteItemAsync(request).ConfigureAwait(false);
			}
		}
	}
}
