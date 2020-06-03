using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.Infrastructure.Models;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Infrastructure
{
    public class DealRepository : IDealRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<DealRepository> _logger;

		public DealRepository(
			AmazonDynamoDBClient client,
			ILogger<DealRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<Deal> CreateAsync(
			Deal dealToCreate)
		{
			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = dealToCreate.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return dealToCreate;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("Deal with this ID already exists.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Deal>> FetchDealsForDateAsync(
			DateTime date,
			string lastSeen = "",
			int limit = 25)
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#pk", "GSI1PK");
			attributeNames.Add("#sk", "GSI1SK");

			var attributeValues = new Dictionary<string, AttributeValue>();
			attributeValues.Add(":pk", new AttributeValue($"DEALS#2020-05-18 00:00:00"));
			attributeValues.Add(":sk", new AttributeValue($"DEAL#{lastSeen}"));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   IndexName = "GSI1",
									   KeyConditionExpression = "#pk = :pk AND #sk < :sk",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false,
									   Limit = limit,
								   };

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var dealResponse = new List<Deal>();

			foreach (var item in queryResult.Items)
			{
				dealResponse.Add(DynamoHelper.CreateFromItem<Deal>(item));
			}

			return dealResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Deal>> FetchDealsForBrandAndDateAsync(
			string brand,
			DateTime date,
			string lastSeen = "$",
			int limit = 25)
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#pk", "GSI2PK");
			attributeNames.Add("#sk", "GSI2SK");

			var attributeValues = new Dictionary<string, AttributeValue>();
			attributeValues.Add(":pk", new AttributeValue($"BRAND#{brand}#{date.ToString("yyyy-MM-dd")} 00:00:00"));
			attributeValues.Add(":sk", new AttributeValue($"DEAL#{lastSeen}"));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   IndexName = "GSI2",
									   KeyConditionExpression = "#pk = :pk AND #sk < :sk",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false,
									   Limit = limit,
								   };

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var dealResponse = new List<Deal>();

			foreach (var item in queryResult.Items)
			{
				dealResponse.Add(DynamoHelper.CreateFromItem<Deal>(item));
			}

			return dealResponse;
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Deal>> FetchDealsForCategoryAndDateAsync(
			string category,
			DateTime date,
			string lastSeen = "$",
			int limit = 25)
		{
			var attributeNames = new Dictionary<string, string>(2);
			attributeNames.Add("#pk", "GSI3PK");
			attributeNames.Add("#sk", "GSI3SK");

			var attributeValues = new Dictionary<string, AttributeValue>();
			attributeValues.Add(":pk", new AttributeValue($"CATEGORY#{category}#{date.ToString("yyyy-MM-dd")} 00:00:00"));
			attributeValues.Add(":sk", new AttributeValue($"DEAL#{lastSeen}"));

			var queryRequest = new QueryRequest()
								   {
									   TableName = DynamoDbConstants.TableName,
									   IndexName = "GSI3",
									   KeyConditionExpression = "#pk = :pk AND #sk < :sk",
									   ExpressionAttributeNames = attributeNames,
									   ExpressionAttributeValues = attributeValues,
									   ScanIndexForward = false,
									   Limit = limit,
								   };

			var queryResult = await this._client.QueryAsync(queryRequest).ConfigureAwait(false);

			var dealResponse = new List<Deal>();

			foreach (var item in queryResult.Items)
			{
				dealResponse.Add(DynamoHelper.CreateFromItem<Deal>(item));
			}

			return dealResponse;
		}

		/// <inheritdoc />
		public async Task<Deal> GetDealAsync(
			string dealId)
		{
			var deal = new Deal()
			{
				DealId = dealId
			};

			var item = await this._client.GetItemAsync(
				           DynamoDbConstants.TableName,
				           deal.AsKeys()).ConfigureAwait(false);

			if (item.IsItemSet)
			{
				return DynamoHelper.CreateFromItem<Deal>(item.Item);
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc />
		public async Task UpdateEditorsChoiceAsync(
			IEnumerable<Deal> deals)
		{
			var transactWriteItemRequest = new TransactWriteItemsRequest();

			for (int x = 0; x < DynamoDbConstants.EditorsChoiceShards; x++)
			{
				transactWriteItemRequest.TransactItems.Add(
					new TransactWriteItem()
						{
							Put = new Put()
									  {
										  TableName = DynamoDbConstants.TableName,
										  Item = EditorsChoice.AsItem(
											  shard: x,
											  deals),
									  }
						});
			}

			await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task UpdateFrontPageAsync(
			IEnumerable<Deal> deals)
		{
			var transactWriteItemRequest = new TransactWriteItemsRequest();

			for (int x = 0; x < DynamoDbConstants.FrontPageShards; x++)
			{
				transactWriteItemRequest.TransactItems.Add(
					new TransactWriteItem()
						{
							Put = new Put()
									  {
										  TableName = DynamoDbConstants.TableName,
										  Item = FrontPage.AsItem(
											  shard: x,
											  deals),
									  }
						});
			}

			await this._client.TransactWriteItemsAsync(transactWriteItemRequest).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Deal>> GetEditorsChoiceAsync()
		{
			var random = new Random(DateTime.Now.Second);
			var randomShard = random.Next(
				0,
				DynamoDbConstants.EditorsChoiceShards);

			var getItemResponse = await this._client.GetItemAsync(
									  new GetItemRequest()
										  {
											  TableName = DynamoDbConstants.TableName,
											  Key = EditorsChoice.AsKeys(randomShard)
										  }).ConfigureAwait(false);

			if (getItemResponse.Item.Any())
			{
				return JsonConvert.DeserializeObject<List<Deal>>(getItemResponse.Item["Data"].S);
			}
			else
			{
				return new List<Deal>();
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<Deal>> GetFrontPageAsync()
		{
			var random = new Random(DateTime.Now.Second);
			var randomShard = random.Next(
				0,
				DynamoDbConstants.FrontPageShards);

			var getItemResponse = await this._client.GetItemAsync(
									  new GetItemRequest()
										  {
											  TableName = DynamoDbConstants.TableName,
											  Key = FrontPage.AsKeys(randomShard)
										  }).ConfigureAwait(false);

			if (getItemResponse.Item.Any())
			{
				return JsonConvert.DeserializeObject<List<Deal>>(getItemResponse.Item["Data"].S);
			}
			else
			{
				return new List<Deal>();
			}
		}
	}
}
