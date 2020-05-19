using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.BigDeals.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController : ControllerBase
    {
		private readonly ILogger<UserController> _logger;

		private readonly IUserRepository _userRepository;

		public UserController(ILogger<UserController> logger,
			IUserRepository userRepository)
		{
			_logger = logger;
			this._userRepository = userRepository;
		}

		[HttpPost]
		public async Task<User> Create(
			[FromBody] UserDTO user)
		{
			return await this._userRepository.CreateAsync(Domain.Entities.User.Create(user.Username))
					   .ConfigureAwait(false);
		}

		[HttpGet]
		public async Task<IEnumerable<User>> List()
		{
			return await this._userRepository.GetAllUsersAsync().ConfigureAwait(false);
		}
    }
}
