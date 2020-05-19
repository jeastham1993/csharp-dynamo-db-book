using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.BigDeals.Controllers
{
	[ApiController]
	[Route("[controller]")]
    public class MessageController : ControllerBase
    {
		private readonly ILogger<MessageController> _logger;

		private readonly IMessageRepository _messageRepository;

		public MessageController(ILogger<MessageController> logger,
			IMessageRepository messageRepository)
		{
			_logger = logger;
			this._messageRepository = messageRepository;
		}

		[HttpGet("{username}")]
		public async Task<IEnumerable<Message>> GetForUsers(
			string username,
			bool onlyUnread = false)
		{
			return await this._messageRepository.FindAllForUserAsync(
					   username,
					   onlyUnread).ConfigureAwait(false);
		}	

		[HttpPut()]
		public async Task<IActionResult> MarkRead(
			[FromBody] Message message)
		{
			await this._messageRepository.MarkMessageAsRead(
					   message).ConfigureAwait(false);

			return this.Ok();
		}
	}
}
