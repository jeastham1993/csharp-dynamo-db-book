using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace DynamoDbBook.GitHub.Controllers
{
	[ApiController]
	[Route("[controller]")]
    public class UserController : ControllerBase
	{
		private readonly IUserRepository _userRepo;

		private readonly IRepoRepository _repoRepository;

		public UserController(
			IUserRepository userRepo,
			IRepoRepository repoRepository)
		{
			this._userRepo = userRepo;
			this._repoRepository = repoRepository;
		}

		[HttpGet("organization/{organizationName}")]
		public async Task<IActionResult> GetOrganization(
			string organizationName)
		{
			return new OkObjectResult(await this._userRepo.GetOrganizationAsync(organizationName).ConfigureAwait(false));
		}

		[HttpGet("organization/{organizationName}/members")]
		public async Task<IActionResult> GetOrganizationMembers(
			string organizationName)
		{
			return new OkObjectResult(await this._userRepo.GetUsersForOrganizationAsync(organizationName).ConfigureAwait(false));
		}

		[HttpGet("organization/{organizationName}/repos")]
		public async Task<IActionResult> GetOrganizationRepos(
			string organizationName)
		{
			return new OkObjectResult(
				await this._repoRepository.GetForOrganizationAsync(organizationName).ConfigureAwait(false));
		}

		[HttpPut("organization/{organizationName}/paymentplan")]
		public async Task<IActionResult> UpdatePaymentPlanForOrganization(
			string organizationName,
			[FromBody] UpdatePaymentPlanDTO paymentPlan)
		{
			await this._userRepo.UpdatePaymentPlanForOrganization(
				organizationName,
				new PaymentPlan(paymentPlan.PlanType, DateTime.Now));

			return this.Ok();
		}

		[HttpPut("user/{userName}/paymentplan")]
		public async Task<IActionResult> UpdatePaymentPlanForUser(
			string userName,
			[FromBody] UpdatePaymentPlanDTO paymentPlan)
		{
			await this._userRepo.UpdatePaymentPlanForUser(
				userName,
				new PaymentPlan(paymentPlan.PlanType, DateTime.Now));

			return this.Ok();
		}

		[HttpPost("organization/{organizationName}")]
		public async Task<IActionResult> AddMemberToOrganization(
			string organizationName,
			[FromBody] AddMemberToOrgDTO member)
		{
			await this._userRepo.AddUserToOrganizationAsync(
				new Membership()
				{
					MemberSince = DateTime.Now,
					OrganizationName = organizationName,
					Role = member.Role,
					Username = member.Username
				});

			return this.Ok();
		}

		[HttpPost("organization")]
		public async Task<IActionResult> CreateOrganization(
			[FromBody] CreateOrganizationDTO org)
		{
			return new OkObjectResult(
				await this._userRepo.CreateOrganizationAsync(
					new Organization() { Name = org.OrganizationName, OwnerName = org.OwnerName }));
		}

		[HttpGet("user/{userName}")]
		public async Task<IActionResult> GetUser(
			string userName)
		{
			return new OkObjectResult(await this._userRepo.GetUserAsync(userName).ConfigureAwait(false));
		}

		[HttpGet("user/{userName}/repos")]
		public async Task<IActionResult> GetUserRepos(
			string userName)
		{
			return new OkObjectResult(await this._repoRepository.GetForUserAsync(userName).ConfigureAwait(false));
		}

		[HttpPost("user")]
		public async Task<IActionResult> CreateUser(
			[FromBody] CreateUserDTO user)
		{
			return new OkObjectResult(
				await this._userRepo.CreateUserAsync(new User() { Name = user.Name, Username = user.Username }));
		}
	}
}
