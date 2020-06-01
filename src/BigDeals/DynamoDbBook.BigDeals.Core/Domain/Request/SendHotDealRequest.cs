using System;
using System.Collections.Generic;
using System.Text;

namespace DynamoDbBook.BigDeals.Core.Domain.Request
{
    public class SendHotDealRequest
    {
	    public string Subject { get; set; }

	    public string Message { get; set; }

	    public string DealId { get; set; }
    }
}
