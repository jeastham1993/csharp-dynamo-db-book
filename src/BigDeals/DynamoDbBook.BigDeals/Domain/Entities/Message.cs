using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public class Message
    {
        public static Message Create(string username, string subject, string body)
		{
			Guard.AgainstNullOrEmpty(
				username,
				nameof(username));

			Guard.AgainstNullOrEmpty(
				subject,
				nameof(subject));
			Guard.AgainstNullOrEmpty(
				body,
				nameof(body));

			return new Message()
					   {
						   MessageId = Ksuid.Generate().ToString().Replace(
							   "/",
							   string.Empty),
						   CreatedAt = DateTime.Now,
						   Username = username,
						   Subject = subject,
						   Body = body,
						   Unread = true
					   };
		}

        [JsonProperty]
        public string MessageId { get; private set; }

		[JsonProperty]
		public DateTime CreatedAt { get; private set; }

		public string Username { get; set; }

        public string Subject { get; set; }

		public string Body { get; set; }

		public bool Unread { get; set; }
	}
}
