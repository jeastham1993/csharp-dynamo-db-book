using DynamoDbBook.BigDeals.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Infrastructure.Models;

using Microsoft.Extensions.Logging;

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
			brandContainerAttributeNames.Add("#brands", "Brands");

			var brandContainerAttributeValues = new Dictionary<string, AttributeValue>(1);
			brandContainerAttributeValues.Add(
				":brand",
				new AttributeValue(new List<string>(1) { brandToCreate.BrandName }));

			for (int shard = 0; shard < DynamoDbConstants.BrandShards; shard++)
			{
				transactWriteItemRequest.TransactItems.Add(new TransactWriteItem()
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
	}
}
