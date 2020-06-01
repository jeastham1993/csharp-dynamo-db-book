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

		[HttpGet("users/{username}/messages")]
		public async Task<IEnumerable<Message>> GetForUsers(
			string username,
			bool onlyUnread = false)
		{
			return await this._messageRepository.FindAllForUserAsync(
					   username,
					   onlyUnread).ConfigureAwait(false);
		}	

		[HttpPost("users/{username}/messages/{messageId}")]
		public async Task<IActionResult> MarkRead(
			string username,
			string messageId)
		{
			await this._messageRepository.MarkMessageAsRead(
					   username,
					   messageId).ConfigureAwait(false);

			return this.Ok();
		}
	}
}
