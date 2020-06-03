using DynamoDbBook.BigDeals.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Infrastructure.Models;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<CategoryRepository> _logger;

		public CategoryRepository(
			AmazonDynamoDBClient client,
			ILogger<CategoryRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Category> CreateOrUpdateAsync(
			Category categoryToCreate)
		{
			var transactWriteItemRequest = new TransactWriteItemsRequest()
											   {
												   TransactItems = new List<TransactWriteItem>(2)
																	   {
																		   new TransactWriteItem()
																			   {
																				   Put = new Put()
																							 {
																								 Item =
																									 categoryToCreate
																										 .AsItem(),
																								 TableName =
																									 DynamoDbConstants
																										 .TableName,
																								 ConditionExpression =
																									 "attribute_not_exists(PK)"
																							 }
																			   },
																	   }
											   };

			var result = await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);

			return categoryToCreate;
		}

		/// <inheritdoc />
		public async Task<Category> GetCategoryAsync(
			string categoryName)
		{
			var getItemRequest = new GetItemRequest(DynamoDbConstants.TableName, new Category(){Name = categoryName}.AsKeys());

			var getItem = await this._client.GetItemAsync(getItemRequest).ConfigureAwait(false);

			return DynamoHelper.CreateFromItem<Category>(getItem.Item);
		}

		/// <inheritdoc />
		public async Task LikeCategoryAsync(
			string categoryName,
			string username)
		{
			var category = new Category()
			{
				Name = categoryName
			};

			var transactWriteItemRequest = new TransactWriteItemsRequest()
											   {
												   TransactItems = new List<TransactWriteItem>(2)
																	   {
																		   new TransactWriteItem()
																			   {
																				   Put = new Put()
																							 {
																								 TableName =
																									 DynamoDbConstants
																										 .TableName,
																								 ConditionExpression =
																									 "attribute_not_exists(PK)",
																								 Item = CategoryLike
																									 .GenerateKeys(
																										 category
																											 .Name,
																										 username)
																							 }
																			   },
																		   new TransactWriteItem()
																			   {
																				   Update = new Update()
																								{
																									TableName =
																										DynamoDbConstants
																											.TableName,
																									ConditionExpression
																										= "attribute_exists(PK)",
																									Key =
																										category.AsKeys(),
																									UpdateExpression =
																										"ADD #data.#likes :incr",
																									ExpressionAttributeNames
																										= CategoryLike
																											.GenerateExpressionAttributeNames(),
																									ExpressionAttributeValues
																										= CategoryLike
																											.GenerateExpressionAttributeValues()
																								}
																			   }
																	   }
											   };

			try
			{
				await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException)
			{
				this._logger.LogWarning($"User {username} has tried to like {category}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task WatchCategoryAsync(
			string categoryName,
			string username)
		{
			var category = new Category()
			{
				Name = categoryName
			};

			var transactWriteItemRequest = new TransactWriteItemsRequest()
											   {
												   TransactItems = new List<TransactWriteItem>(2)
																	   {
																		   new TransactWriteItem()
																			   {
																				   Put = new Put()
																							 {
																								 TableName =
																									 DynamoDbConstants
																										 .TableName,
																								 ConditionExpression =
																									 "attribute_not_exists(PK)",
																								 Item = CategoryWatch
																									 .GenerateKeys(
																										 category
																											 .Name,
																										 username)
																							 }
																			   },
																		   new TransactWriteItem()
																			   {
																				   Update = new Update()
																								{
																									TableName =
																										DynamoDbConstants
																											.TableName,
																									ConditionExpression
																										= "attribute_exists(PK)",
																									Key =
																										category.AsKeys(),
																									UpdateExpression =
																										"ADD #data.#watcher :incr",
																									ExpressionAttributeNames
																										= CategoryWatch
																											.GenerateExpressionAttributeNames(),
																									ExpressionAttributeValues
																										= CategoryWatch
																											.GenerateExpressionAttributeValues()
																								}
																			   }
																	   }
											   };

			try
			{
				await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);
			}
			catch (ConditionalCheckFailedException)
			{
				this._logger.LogWarning($"User {username} has tried to like {category}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<string>> FindWatchersForCategory(
			string categoryName)
		{
			var userList = new List<string>();

			Dictionary<string, AttributeValue> exclusiveStartKey = null;

			while (true)
			{
				var query = new QueryRequest()
								{
									TableName = DynamoDbConstants.TableName,
									KeyConditionExpression = "#pk = :pk",
									ExpressionAttributeNames = new Dictionary<string, string>(1) { { "#pk", "PK" }, },
									ExpressionAttributeValues = new Dictionary<string, AttributeValue>(1)
																	{
																		{
																			":pk",
																			new AttributeValue($"CATEGORYWATCH#{categoryName}")
																		},
																	}
								};

				if (exclusiveStartKey != null)
				{
					query.ExclusiveStartKey = exclusiveStartKey;
				}

				var queryResults = await this._client.QueryAsync(query).ConfigureAwait(false);

				foreach (var queryResult in queryResults.Items)
				{
					userList.Add(
						queryResult["SK"].S.Replace(
							"USER#",
							string.Empty));
				}

				if (queryResults.LastEvaluatedKey.Count == 0)
				{
					break;
				}
				else
				{
					exclusiveStartKey = queryResults.LastEvaluatedKey;
				}
			}

			return userList;
		}
	}
}