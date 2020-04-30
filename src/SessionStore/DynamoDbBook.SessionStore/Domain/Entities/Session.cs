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
								  SessionToken = Guid.NewGuid(), CreatedAt = DateTime.Now, Username = userName
							  };
		}

		[JsonProperty]
        public Guid SessionToken { get; private set; }

		[JsonProperty]
		public string Username { get; private set; }

		[JsonProperty]
		public DateTime CreatedAt { get; private set; }

		[JsonProperty]
		public DateTime ExpiresAt => this.CreatedAt.AddDays(SessionAliveFor);
	}
}
