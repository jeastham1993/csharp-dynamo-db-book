using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.GitHubMigration.Core.Domain.Entities;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public interface IUserRepository
	{
		Task<User> CreateUserAsync(
			User user);

		Task<Gist> CreateGist(
			Gist gist);

		Task<User> GetUserAsync(
			string userName);

		Task<IEnumerable<Gist>> GetGistsAsync(
			string userName);

		Task<Organization> CreateOrganizationAsync(
			Organization org);

		Task AddUserToOrganizationAsync(
			Membership membership);

		Task<Organization> GetOrganizationAsync(
			string organizationName);

		Task<List<User>> GetUsersForOrganizationAsync(
			string organizationName);

		Task<Organization> UpdatePaymentPlanForOrganization(
			string organizationName,
			PaymentPlan plan);

		Task<User> UpdatePaymentPlanForUser(
			string userName,
			PaymentPlan plan);
	}
}
