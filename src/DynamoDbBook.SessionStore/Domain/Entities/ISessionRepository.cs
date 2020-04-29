using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.SessionStore.Domain.Entities
{
    public interface ISessionRepository
	{
		Task<Session> CreateSession(
			Session sessionToCreate);

		Task<Session> GetSession(
			Guid token);

		Task DeleteSessionForUser(
			string username);
	}
}
