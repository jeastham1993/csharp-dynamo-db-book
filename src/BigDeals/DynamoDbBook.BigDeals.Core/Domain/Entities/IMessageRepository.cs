using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public interface IMessageRepository
	{
		Task<Message> CreateAsync(
			Message messageToCreate);

		Task<List<Message>> CreateBatchAsync(
			List<Message> messagesToCreate);

		Task<IEnumerable<Message>> FindAllForUserAsync(
			string username,
			bool onlyUnread);

		Task MarkMessageAsRead(
			string username,
			string messageId);
	}
}
