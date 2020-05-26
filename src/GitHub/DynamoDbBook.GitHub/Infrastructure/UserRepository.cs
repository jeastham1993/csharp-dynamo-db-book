using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHub.Domain.Entities;

namespace DynamoDbBook.GitHub.Infrastructure
{
    public class UserRepository : IUserRepository
    {
		/// <inheritdoc />
		public async Task<User> CreateUserAsync(
			User user)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> GetUserAsync(
			string userName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> CreateOrganizationAsync(
			Organization org)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task AddUserToOrganizationAsync(
			Membership membership)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> GetOrganizationAsync(
			string organizationName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<User>> GetUsersForOrganizationAsync(
			string organizationName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<List<Organization>> GetOrganizationsForUserAsync(
			string userName)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<Organization> UpdatePaymentPlanForOrganization(
			string organizationName,
			PaymentPlan plan)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public async Task<User> UpdatePaymentPlanForUser(
			string userName,
			PaymentPlan plan)
		{
			throw new NotImplementedException();
		}
	}
}
