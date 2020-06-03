using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain;
using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.Domain.Entities.Reactions;
using DynamoDbBook.GitHub.ViewModels;
using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

namespace DynamoDbBook.GitHub.Controllers
{
	[ApiController]
	public class AppController : ControllerBase
	{
		private readonly IAppRepository _appRepository;

		public AppController(
			IAppRepository appRepository)
		{
			this._appRepository = appRepository;
		}

		[HttpPost("apps")]
		public async Task<IActionResult> CreateApp(
			[FromBody] GitHubApp app)
		{
			return new OkObjectResult(
				await this._appRepository.CreateAppAsync(app));
		}

		[HttpPost("apps/{appOwner}/{appName}/installations")]
		public async Task<IActionResult> InstallApp(
			string appOwner,
			string appName,
			[FromBody] GitHubAppInstallation app)
		{
			app.AppOwner = appOwner;
			app.AppName = appName;

			var install = await this._appRepository.InstallAppAsync(app).ConfigureAwait(false);

			return new OkObjectResult(install);
		}

		[HttpGet("apps/{appOwner}/{appName}/installations")]
		public async Task<IActionResult> GetInstalls(
			string appOwner,
			string appName)
		{
			var installs = await this._appRepository.GetAppInstallations(
				               appOwner,
				               appName).ConfigureAwait(false);

			return new OkObjectResult(installs);
		}
	}
}