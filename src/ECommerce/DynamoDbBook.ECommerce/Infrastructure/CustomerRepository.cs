using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.Infrastructure.Models;
using DynamoDbBook.SessionStore.Infrastructure;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<CustomerRepository> _logger;

		public CustomerRepository(
			AmazonDynamoDBClient client,
			ILogger<CustomerRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Customer> GetCustomerAsync(
			string username)
		{
			var keys = new Dictionary<string, AttributeValue>(2);

			keys.Add("PK", new AttributeValue($"CUSTOMER#{username}"));
			keys.Add("SK", new AttributeValue($"CUSTOMER#{username}"));

			var getItemRequest = new GetItemRequest() { TableName = DynamoDbConstants.TableName, Key = keys };

			var getResult = await this._client.GetItemAsync(getItemRequest).ConfigureAwait(false);

			if (getResult.Item != null)
			{
				var customerData = getResult.Item.FirstOrDefault(p => p.Key == "Data");

				var addressDetails = customerData.Value.M.FirstOrDefault(p => p.Key == "addresses");

				customerData.Value.M.Remove("addresses");

				var customer = JsonConvert.DeserializeObject<Customer>(Document.FromAttributeMap(customerData.Value.M).ToJson());

				foreach (var address in addressDetails.Value.M)
				{
					customer.AddAddress(
						address.Key,
						address.Value.M["StreetAddress"].S,
						address.Value.M["PostalCode"].S,
						address.Value.M["Country"].S);
				}

				return customer;
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc />
		public async Task<Customer> CreateAsync(
			Customer customerToCreate)
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
																						 Item =
																							 customerToCreate.AsItem()
																					 }
																	   },
																   new TransactWriteItem()
																	   {
																		   Put = new Put()
																					 {
																						 ConditionExpression =
																							 "attribute_not_exists(PK)",
																						 TableName = DynamoDbConstants
																							 .TableName,
																						 Item =
																							 customerToCreate.AsCustomerEmailItem()
																					 }
																	   }
															   }
									   };

			try
			{
				await this._client.TransactWriteItemsAsync(writeItemRequest).ConfigureAwait(false);

				return customerToCreate;
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
		public async Task AddAddressAsync(
			string username,
			Address address)
		{
			try
			{
				var updateKey = new Dictionary<string, AttributeValue>(2);
				updateKey.Add(
					"PK",
					new AttributeValue($"CUSTOMER#{username}"));
				updateKey.Add(
					"SK",
					new AttributeValue($"CUSTOMER#{username}"));

				var expressionAttributeNames = new Dictionary<string, string>(3);
				expressionAttributeNames.Add(
					"#data",
					"Data");
				expressionAttributeNames.Add(
					"#addresses",
					"addresses");
				expressionAttributeNames.Add(
					"#name",
					address.Name);

				var updateValues = new Dictionary<string, AttributeValue>(1);
				updateValues.Add(
					":address",
					new AttributeValue() { M = address.AsItem(), });

				var updateItemRequest = new UpdateItemRequest()
											{
												TableName = DynamoDbConstants.TableName,
												Key = updateKey,
												ConditionExpression = "attribute_exists(PK)",
												UpdateExpression = "SET #data.#addresses.#name = :address",
												ExpressionAttributeNames = expressionAttributeNames,
												ExpressionAttributeValues = updateValues,
												ReturnValues = ReturnValue.ALL_NEW
											};

				var updateItemResponse = await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);

				if (updateItemResponse.HttpStatusCode != HttpStatusCode.OK)
				{
					throw new Exception("Failure updating item");
				}
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(
					ex,
					"Customer does not exist");

				throw;
			}
		}

		public async Task DeleteAddressAsync(
			string username,
			string addressName)
		{
			try
			{
				var updateKey = new Dictionary<string, AttributeValue>(2);
				updateKey.Add(
					"PK",
					new AttributeValue($"CUSTOMER#{username}"));
				updateKey.Add(
					"SK",
					new AttributeValue($"CUSTOMER#{username}"));

				var expressionAttributeNames = new Dictionary<string, string>(3);
				expressionAttributeNames.Add(
					"#data",
					"Data");
				expressionAttributeNames.Add(
					"#addresses",
					"addresses");
				expressionAttributeNames.Add(
					"#name",
					addressName);

				var updateItemRequest = new UpdateItemRequest()
											{
												TableName = DynamoDbConstants.TableName,
												Key = updateKey,
												ConditionExpression = "attribute_exists(PK)",
												UpdateExpression = "REMOVE #data.#addresses.#name",
												ExpressionAttributeNames = expressionAttributeNames,
												ReturnValues = ReturnValue.ALL_NEW
											};

				var updateItemResponse = await this._client.UpdateItemAsync(updateItemRequest).ConfigureAwait(false);

				if (updateItemResponse.HttpStatusCode != HttpStatusCode.OK)
				{
					throw new Exception("Failure deleting item");
				}
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError(
					ex,
					"Customer does not exist");

				throw;
			}
		}
	}
}
