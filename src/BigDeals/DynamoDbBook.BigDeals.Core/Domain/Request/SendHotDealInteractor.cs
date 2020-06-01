using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;

namespace DynamoDbBook.BigDeals.Core.Domain.Request
{
    public class SendHotDealInteractor
    {
	    private readonly IUserRepository _userRepository;
	    private readonly IDealRepository _dealRepository;
	    private readonly IMessageRepository _messageRepository;

	    public SendHotDealInteractor(
		    IUserRepository userRepository,
		    IDealRepository dealRepository,
		    IMessageRepository messageRepository)
	    {
		    this._userRepository = userRepository;
		    this._dealRepository = dealRepository;
		    this._messageRepository = messageRepository;
	    }

	    public async Task Handle(
		    SendHotDealRequest hotDealRequest)
	    {
		    var existingDeal = await this._dealRepository.GetDealAsync(hotDealRequest.DealId).ConfigureAwait(false);

		    if (existingDeal != null)
		    {
			    var users = await this._userRepository.GetAllUsersAsync().ConfigureAwait(false);

			    var messages = new List<Message>();

			    foreach (var user in users)
			    {
				    messages.Add(
					    Message.Create(
						    user.Username,
						    hotDealRequest.Subject,
						    hotDealRequest.Message));
			    }

			    await this._messageRepository.CreateBatchAsync(messages).ConfigureAwait(false);
		    }
	    }
    }
}
