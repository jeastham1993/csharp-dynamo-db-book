﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.Infrastructure.Models;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<OrderRepository> _logger;

		public OrderRepository(
			AmazonDynamoDBClient client,
			ILogger<OrderRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Order> GetOrderAsync(
			string username,
			string orderId)
		{
			var attributeNames = new Dictionary<string, string>(1);
			attributeNames.Add("#gsi1pk", "GSI1PK");

			var attributeValues = new Dictionary<string, AttributeValue>(1);
			attributeValues.Add(":gsi1pk", new AttributeValue($"ORDER#{orderId}"));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   IndexName = "GSI1",
									   KeyConditionExpression = "#gsi1pk = :gsi1pk",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false
								   };

			var getResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			if (getResult.Items.Count > 0)
			{
				var order = DynamoHelper.CreateFromItem<Order>(getResult.Items[0]);

				for (var x = 1; x < getResult.Items.Count; x++)
				{
					order.AddItem(DynamoHelper.CreateFromItem<OrderItem>(getResult.Items[x]));
				}

				return order;
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Order> CreateAsync(
			Order order)
		{
			var writeItemRequest = new TransactWriteItemsRequest()
									   {
										   TransactItems = new List<TransactWriteItem>(2)
															   {
																   new TransactWriteItem()
																	   {
																		   Put = new Put()
																					 {
																						 ConditionExpression =
																							 "attribute_not_exists(PK)",
																						 TableName = DynamoDbConstants
																							 .TableName,
																						 Item = order.AsItem()
																					 }
																	   }
															   }
									   };

			foreach (var item in order.Items)
			{
				writeItemRequest.TransactItems.Add(
					new TransactWriteItem()
						{
							Put = new Put()
									  {
										  TableName = DynamoDbConstants.TableName,
										  Item = item.AsAttributeMap()
									  }
						});
			}

			try
			{
				await this._client.TransactWriteItemsAsync(writeItemRequest).ConfigureAwait(false);

				return order;
			}
			catch (TransactionCanceledException ex)
			{
				this._logger.LogError(ex, "Failure creating customer");
				
				return null;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(ex, "Error creating customer");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Order>> GetOrdersForCustomerAsync(
			string username)
		{
			var attributeNames = new Dictionary<string, string>(1);
			attributeNames.Add("#pk", "PK");

			var attributeValues = new Dictionary<string, AttributeValue>(1);
			attributeValues.Add(":pk", new AttributeValue($"CUSTOMER#{username}"));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   KeyConditionExpression = "#pk = :pk",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false
								   };

			var getResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var orders = new List<Order>();

			if (getResult.Items.Count > 0)
			{
				foreach (var item in getResult.Items)
				{
					if (item["Type"].S == "Order")
					{
						var orderData = item.FirstOrDefault(p => p.Key == "Data");

						orders.Add(
							JsonConvert.DeserializeObject<Order>(Document.FromAttributeMap(orderData.Value.M).ToJson()));
					}
				}

				return orders;
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Order> UpdateOrderStatusAsync(
			string username,
			string orderId,
			OrderStatus newStatus)
		{
			var order = new Order()
			{
				OrderId = orderId,
				Username = username
			};

			var attributeNames = new Dictionary<string, string>(1);
			attributeNames.Add(
				"#data",
				"Data");
			attributeNames.Add(
				"#status",
				"Status");

			var attributeValues = new Dictionary<string, AttributeValue>(1);
			attributeValues.Add(
				":status",
				new AttributeValue(newStatus.ToString()));

			var updateItemRequest = new UpdateItemRequest()
			{
				TableName = DynamoDbConstants.TableName,
				Key = order.AsKeys(),
				ConditionExpression = "attribute_exists(PK)",
				UpdateExpression = "SET #data.#status = :status",
				ExpressionAttributeNames = attributeNames,
				ExpressionAttributeValues = attributeValues,
				ReturnValues = "ALL_NEW"
			};

			try
			{
				var updatedItem = await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);

				return DynamoHelper.CreateFromItem<Order>(updatedItem.Attributes);
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(ex, $"Order {order.OrderId} does not exist for user {username}");

				return null;
			}
		}
	}
}
