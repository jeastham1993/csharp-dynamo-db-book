using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.SessionStore.ViewModels
{
    public class CreateSessionResponse
    {
		public Guid SessionId { get; set; }

		public DateTime ExpiresAt { get; set; }
    }
}
