using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.SessionStore.Domain.Entities;

namespace DynamoDbBook.SessionStore.Infrastructure
{
    public static class SessionExtensions
    {
        public static Dictionary<string, AttributeValue> AssAttributeMap(this Session session)
        {
			if (session == null)
			{
				throw new ArgumentNullException(nameof(session));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("SessionId", new AttributeValue(session.SessionId.ToString()));
			attributeMap.Add("Username", new AttributeValue(session.Username));
			attributeMap.Add("CreatedAt", new AttributeValue(session.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")));
			attributeMap.Add("ExpiresAt", new AttributeValue(session.ExpiresAt.ToString("yyyy-MM-ddTHH:mm:ssZ")));
			attributeMap.Add("TTL", new AttributeValue()
										{
											N = ToEpoch(session.ExpiresAt).ToString()
										});

			return attributeMap;
		}

		public static long ToEpoch(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
	}
}
