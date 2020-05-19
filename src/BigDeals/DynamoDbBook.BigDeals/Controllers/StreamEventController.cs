using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.BigDeals.Controllers
{
    /// <summary>
    /// This controller is used to mock the notification of a new DynamoDB Stream event.
    /// </summary>
	[ApiController]
	[Route("[controller]")]
    public class StreamEventController
    {
		private readonly ILogger<StreamEventController> _logger;

		private readonly IBrandRepository _brandRepository;

		private readonly ICategoryRepository _categoryRepository;

		private readonly IMessageRepository _messageRepository;

		public StreamEventController(ILogger<StreamEventController> logger,
			IBrandRepository brandRepository,
			ICategoryRepository categoryRepository,
			IMessageRepository messageRepository)
		{
			_logger = logger;
			this._brandRepository = brandRepository;
			this._categoryRepository = categoryRepository;
			this._messageRepository = messageRepository;
		}

		[HttpPut("brand/{brandName}")]
		public async Task<List<string>> NewDealAddedForBrand(
			string brandName)
		{
			var users = await this._brandRepository.FindWatchersForBrand(brandName).ConfigureAwait(false);

			var eventsRaised = new List<string>(users.Count());

			var tasks = new List<Task>(users.Count());

			foreach (var user in users)
			{
				this._logger.LogInformation($"Sending new deal event to {user}");

				tasks.Add(
					this._messageRepository.CreateAsync(
						Message.Create(
							user,
							$"New deal for {brandName}",
							$"There is a new deal for {brandName}")));

				eventsRaised.Add($"Sending new deal event to {user}");
			}

			Task.WaitAll(tasks.ToArray());

			return eventsRaised;
		}

		[HttpPut("category/{categoryName}")]
		public async Task<List<string>> NewDealAddedForCategory(
			string categoryName)
		{
			var users = await this._categoryRepository.FindWatchersForCategory(categoryName).ConfigureAwait(false);

			var eventsRaised = new List<string>(users.Count());

			var tasks = new List<Task>(users.Count());

			foreach (var user in users)
			{
				this._logger.LogInformation($"Sending new deal event to {user}");

				tasks.Add(
					this._messageRepository.CreateAsync(
						Message.Create(
							user,
							$"New deal for {categoryName}",
							$"There is a new deal for {categoryName}")));

				eventsRaised.Add($"Sending new deal event to {user}");
			}

			Task.WaitAll(tasks.ToArray());

			return eventsRaised;
		}
	}
}
