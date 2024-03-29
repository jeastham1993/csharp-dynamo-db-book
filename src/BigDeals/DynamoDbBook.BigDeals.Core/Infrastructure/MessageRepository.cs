﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.Infrastructure.Models;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure
{
    public class MessageRepository : IMessageRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<MessageRepository> _logger;

		public MessageRepository(
			AmazonDynamoDBClient client,
			ILogger<MessageRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Message> CreateAsync(
			Message messageToCreate)
		{
			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = messageToCreate.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return messageToCreate;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Message with this ID already exists.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<List<Message>> CreateBatchAsync(
			List<Message> messagesToCreate)
		{
			var writeRequest = new List<WriteRequest>();

			var requestCounter = 25;

			foreach (var message in messagesToCreate)
			{
				writeRequest.Add(new WriteRequest(new PutRequest(message.AsItem())));

				requestCounter--;

				if (requestCounter == 0)
				{
					requestCounter = 25;

					await this.MakeBatchRequest(writeRequest).ConfigureAwait(false);

					writeRequest = new List<WriteRequest>();
				}
			}

			if (writeRequest.Any())
			{
				await this.MakeBatchRequest(writeRequest).ConfigureAwait(false);
			}

			return messagesToCreate;
		}

		private async Task MakeBatchRequest(List<WriteRequest> writeRequests)
		{
			// Individual items within a batch can fail, so we'll attempt
			// to reprocess them 5 times.
			for (var x = 5; x > 0; x--)
			{
				var batchWriteResult = await this._client.BatchWriteItemAsync(
					                       new BatchWriteItemRequest(
						                       new Dictionary<string, List<WriteRequest>>(1)
						                       {
							                       { DynamoDbConstants.TableName, writeRequests }
						                       }));

				if (batchWriteResult.UnprocessedItems.Count == 0)
				{
					break;
				}
				else
				{
					writeRequests = batchWriteResult.UnprocessedItems.FirstOrDefault().Value;
				}
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Message>> FindAllForUserAsync(
			string username,
			bool onlyUnread)
		{
			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   KeyConditionExpression = "#pk = :pk",
									   ExpressionAttributeNames = new Dictionary<string, string>(1)
																	  {
																		  { "#pk", "PK" },
																	  },
									   ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
									   {
										{ ":pk", new AttributeValue($"MESSAGE#{username}") }
									   },
									   ScanIndexForward = false
								   };

			if (onlyUnread)
			{
				queryRequest.IndexName = "GSI1";
				queryRequest.ExpressionAttributeNames = new Dictionary<string, string>(1)
															{
																{ "#pk", "GSI1PK" },
															};
			}

			var results = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var messages = new List<Message>(results.Count);

			foreach (var result in results.Items)
			{
				var messageData = result.FirstOrDefault(p => p.Key == "Data");

				if (messageData.Value == null)
				{
					continue;
				}

				var message =
					JsonConvert.DeserializeObject<Message>(Document.FromAttributeMap(messageData.Value.M).ToJson());

				if (result.ContainsKey("Unread"))
				{
					message.Unread = result["Unread"].BOOL;
				}

				messages.Add(message);
			}

			return messages;
		}

		/// <inheritdoc />
		public async Task MarkMessageAsRead(string username, string messageId)
		{
			var message = new Message()
			{
				Username = username,
				MessageId = messageId
			};

			var updateItemRequest = new UpdateItemRequest()
										{
											TableName = DynamoDbConstants.TableName,
											Key = message.AsKeys(),
											UpdateExpression = "SET #unread = :false REMOVE #gsi1pk, #gsi1sk",
											ExpressionAttributeNames = new Dictionary<string, string>(3)
																		   {
																			   { "#unread", "Unread" },
																			   { "#gsi1pk", "GSI1PK" },
																			   { "#gsi1sk", "GSI1SK" }
																		   },
											ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																			{
																				{
																					":false", new AttributeValue(){ BOOL = false}
																				},
																			}
										};

			await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);
		}
	}
}
