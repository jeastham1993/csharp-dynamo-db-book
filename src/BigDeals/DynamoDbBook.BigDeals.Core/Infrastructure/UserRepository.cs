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
    public class UserRepository : IUserRepository
    {
		private readonly AmazonDynamoDBClient _client;

		private readonly ILogger<UserRepository> _logger;

		public UserRepository(
			AmazonDynamoDBClient client,
			ILogger<UserRepository> logger)
		{
			this._client = client;
			this._logger = logger;
		}

		/// <inheritdoc />
		public async Task<User> CreateAsync(
			User userToCreate)
		{
			var putItemRequest = new PutItemRequest()
									 {
										 TableName = DynamoDbConstants.TableName,
										 Item = userToCreate.AsItem(),
										 ConditionExpression = "attribute_not_exists(PK)"
									 };

			try
			{
				await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

				return userToCreate;
			}
			catch (ConditionalCheckFailedException ex)
			{
				this._logger.LogError("User with this name already exists.");

				return null;
			}
		}

		/// <inheritdoc />
		public async Task<User> GetUserAsync(
			string username)
		{
			var user = User.Create(username);

			var item = await this._client.GetItemAsync(
				           DynamoDbConstants.TableName,
				           user.AsKeys()).ConfigureAwait(false);

			if (item.IsItemSet)
			{
				return DynamoHelper.CreateFromItem<User>(item.Item);
			}
			else
			{
				return null;
			}
		}

		/// <inheritdoc />
		public async Task<IEnumerable<User>> GetAllUsersAsync()
		{
			var scanRequest = new ScanRequest() { TableName = DynamoDbConstants.TableName, IndexName = "UserIndex" };

			var scanResults = await this._client.ScanAsync(scanRequest).ConfigureAwait(false);

			var users = new List<User>(scanResults.Items.Count);

			foreach (var result in scanResults.Items)
			{
				var userData = result.FirstOrDefault(p => p.Key == "Data");

				users.Add(
					JsonConvert.DeserializeObject<User>(Document.FromAttributeMap(userData.Value.M).ToJson()));
			}

			return users;
		}
	}
}
