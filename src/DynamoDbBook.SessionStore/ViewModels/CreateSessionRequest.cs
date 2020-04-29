using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.SessionStore.ViewModels
{
    public class CreateSessionRequest
    {
		public string Username { get; set; }

        public string Password { get; set; }
    }
}
