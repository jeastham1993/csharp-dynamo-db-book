using DynamoDbBook.BigDeals.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Infrastructure.Models;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure
{
	public class BrandRepository : IBrandRepository
	{
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<BrandRepository> _logger;

		public BrandRepository(
			AmazonDynamoDBClient client,
			ILogger<BrandRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Brand> CreateAsync(
			Brand brandToCreate)
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
																									 brandToCreate
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
			var brandContainerAttributeNames = new Dictionary<string, string>(1);
			brandContainerAttributeNames.Add(
				"#brands",
				"Brands");

			var brandContainerAttributeValues = new Dictionary<string, AttributeValue>(1);
			brandContainerAttributeValues.Add(
				":brand",
				new AttributeValue(new List<string>(1) { brandToCreate.BrandName }));

			for (int shard = 0; shard < DynamoDbConstants.BrandShards; shard++)
			{
				transactWriteItemRequest.TransactItems.Add(
					new TransactWriteItem()
						{
							Update = new Update()
										 {
											 Key = BrandContainer.AsKeys(shard),
											 TableName = DynamoDbConstants.TableName,
											 UpdateExpression = "ADD #brands :brand",
											 ExpressionAttributeNames = brandContainerAttributeNames,
											 ExpressionAttributeValues = brandContainerAttributeValues
										 }
						});
			}

			var result = await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);

			return brandToCreate;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<string>> ListBrandsAsync()
		{
			var random = new Random(DateTime.Now.Second);
			var randomShard = random.Next(
				0,
				DynamoDbConstants.BrandShards);

			var getItemResponse = await this._client.GetItemAsync(
									  new GetItemRequest()
										  {
											  TableName = DynamoDbConstants.TableName,
											  Key = BrandContainer.AsKeys(randomShard)
										  }).ConfigureAwait(false);

			if (getItemResponse.Item.Any())
			{
				return getItemResponse.Item["Brands"].SS;
			}
			else
			{
				return new List<string>();
			}
		}

		/// <inheritdoc />
		public async Task<Brand> GetBrandAsync(
			string brandName)
		{
			var getItemRequest = new GetItemRequest(DynamoDbConstants.TableName, new Brand(){BrandName = brandName}.AsKeys());

			var getItem = await this._client.GetItemAsync(getItemRequest).ConfigureAwait(false);

			var brandData = getItem.Item.FirstOrDefault(p => p.Key == "Data");

			return JsonConvert.DeserializeObject<Brand>(Document.FromAttributeMap(brandData.Value.M).ToJson());
		}

		/// <inheritdoc />
		public async Task LikeBrandAsync(
			Brand brand,
			string username)
		{
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
																								 Item = BrandLike
																									 .GenerateKeys(
																										 brand
																											 .BrandName,
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
																										brand.AsKeys(),
																									UpdateExpression =
																										"ADD #data.#likes :incr",
																									ExpressionAttributeNames
																										= BrandLike
																											.GenerateExpressionAttributeNames(),
																									ExpressionAttributeValues
																										= BrandLike
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
				this._logger.LogWarning($"User {username} has tried to like {brand}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task WatchBrandAsync(
			Brand brand,
			string username)
		{
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
																								 Item = BrandWatch
																									 .GenerateKeys(
																										 brand
																											 .BrandName,
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
																										brand.AsKeys(),
																									UpdateExpression =
																										"ADD #data.#watcher :incr",
																									ExpressionAttributeNames
																										= BrandWatch
																											.GenerateExpressionAttributeNames(),
																									ExpressionAttributeValues
																										= BrandWatch
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
				this._logger.LogWarning($"User {username} has tried to like {brand}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<string>> FindWatchersForBrand(
			string brandName)
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
																			new AttributeValue($"BRANDWATCH#{brandName}")
																		},
																	},
									Limit = 1
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