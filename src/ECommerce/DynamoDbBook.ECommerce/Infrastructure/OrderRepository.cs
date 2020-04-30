using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.Infrastructure.Models;

using Microsoft.Extensions.Logging;

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
																						 Item = order.AsAttributeMap()
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
	}
}
