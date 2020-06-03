using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHubMigration.Core.Domain.Entities;
using DynamoDbBook.GitHubMigration.Core.Infrastructure.Migrations;

using Microsoft.AspNetCore.Mvc;

namespace DynamoDbBook.GitHubMigration.Controllers
{
	[ApiController]
	public class MigrationController : Controller
	{
		private readonly AccountMigration _accountMigration;
		private readonly RepoMigration _repoMigration;

		public MigrationController(
			AccountMigration accountMigration,
			RepoMigration repoMigration)
		{
			this._accountMigration = accountMigration;
			this._repoMigration = repoMigration;
		}

		[HttpPost("migrate/accounts")]
		public async Task<IActionResult> MigrateAccounts()
		{
			await this._accountMigration.Execute().ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPost("migrate/repos")]
		public async Task<IActionResult> MigrateRepos()
		{
			await this._repoMigration.Execute().ConfigureAwait(false);

			return this.Ok();
		}
	}
}