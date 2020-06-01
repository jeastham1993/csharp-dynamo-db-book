using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;

using DynamoDbBook.BigDeals.Core.Domain.Request;
using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.SharedKernel;

using Microsoft.Extensions.DependencyInjection;

namespace DynamoDbBook.BigDeals.Serverless.Streams
{
	public class StreamHandler
	{
		private readonly IBrandRepository _brandRepository;

		private readonly ICategoryRepository _categoryRepository;

		private readonly IMessageRepository _messageRepository;

		public StreamHandler()
		{
			var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
			var serviceProvider = serviceCollection.BuildServiceProvider();

			this._brandRepository = serviceProvider.GetRequiredService<IBrandRepository>();
			this._categoryRepository = serviceProvider.GetRequiredService<ICategoryRepository>();
			this._messageRepository = serviceProvider.GetRequiredService<IMessageRepository>();
		}

		public async Task Handle(
			DynamoDBEvent dynamoDbEvent,
			ILambdaContext context)
		{
			context.Logger.Log("Received new stream event");

			foreach (var record in dynamoDbEvent.Records.Where(
				p => p.EventName == OperationType.INSERT))
			{
				foreach (var key in record.Dynamodb.NewImage)
				{
					context.Logger.Log($"{key.Key} : {key.Value}");
				}

				if (record.Dynamodb.NewImage.ContainsKey("Type") && record.Dynamodb.NewImage["Type"].S == "Deal")
				{
					var deal = DynamoHelper.CreateFromItem<Deal>(record.Dynamodb.NewImage);

					var brandUsers = (await this._brandRepository.FindWatchersForBrand(deal.Brand).ConfigureAwait(false))
						.ToList();
					var categoryUsers = await this._categoryRepository.FindWatchersForCategory(deal.Category).ConfigureAwait(false);

					var eventsRaised = new List<string>();

					var tasks = new List<Task>();

					foreach (var user in brandUsers)
					{
						context.Logger.Log($"Sending new deal event to {user}");

						tasks.Add(
							this._messageRepository.CreateAsync(
								Message.Create(
									user,
									$"New deal for {deal.Brand}",
									$"There is a new deal for {deal.Brand}")));

						eventsRaised.Add($"Sending new deal event to {user}");
					}

					foreach (var user in categoryUsers)
					{
						context.Logger.Log($"Sending new deal event to {user}");

						tasks.Add(
							this._messageRepository.CreateAsync(
								Message.Create(
									user,
									$"New deal for {deal.Category}",
									$"There is a new deal for {deal.Category}")));

						eventsRaised.Add($"Sending new deal event to {user}");
					}

					Task.WaitAll(tasks.ToArray());
				}
			}
		}
	}
}