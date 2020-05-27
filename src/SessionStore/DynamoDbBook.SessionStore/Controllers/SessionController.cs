using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DynamoDbBook.SessionStore.Domain.Entities;
using DynamoDbBook.SessionStore.Domain.Exceptions;
using DynamoDbBook.SessionStore.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.SessionStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
		private readonly ILogger<SessionController> _logger;

		private readonly ISessionRepository _sessionRepository;

        public SessionController(ILogger<SessionController> logger,
			ISessionRepository sessionRepository)
		{
			_logger = logger;
			this._sessionRepository = sessionRepository;
		}

		[HttpGet("{sessionId}")]
		public async Task<ActionResult<Session>> GetSession(Guid sessionId)
		{
			try
			{
				return await this._sessionRepository.GetSession(sessionId)
						   .ConfigureAwait(false);
			}
			catch (InvalidSessionException)
			{
				return this.NotFound();
			}
		}

		[HttpDelete("{username}")]
		public async Task<IActionResult> DeleteSession(string username)
		{
			await this._sessionRepository.DeleteSessionForUser(username)
					   .ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPost]
		public async Task<CreateSessionResponse> CreateSession(
			CreateSessionRequest sessionRequest)
		{
			// Add authentication of password here. 

			var response = await this._sessionRepository.CreateSession(Session.CreateNew(sessionRequest.Username))
							   .ConfigureAwait(false);

			return new CreateSessionResponse()
					   {
						   SessionId = response.SessionId,
						   ExpiresAt = response.ExpiresAt,
					   };
		}
    }
}
