using System;

using Newtonsoft.Json;

namespace DynamoDbBook.SessionStore.Domain.Entities
{
	public class Session
    {
		private const int SessionAliveFor = 7;

		public static Session CreateNew(string userName)
		{
			if (string.IsNullOrEmpty(userName))
			{
				throw new ArgumentException(
					"Username cannot be empty",
					nameof(userName));
			}

			return new Session()
							  {
								  SessionId = Guid.NewGuid(), CreatedAt = DateTime.Now, Username = userName
							  };
		}

		[JsonProperty]
        public Guid SessionId { get; private set; }

		[JsonProperty]
		public string Username { get; private set; }

		[JsonProperty]
		public DateTime CreatedAt { get; private set; }

		[JsonProperty]
		public DateTime ExpiresAt => this.CreatedAt.AddDays(SessionAliveFor);
	}
}
