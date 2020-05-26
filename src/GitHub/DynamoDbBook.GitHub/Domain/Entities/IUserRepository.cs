using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public interface IUserRepository
	{
		Task<User> CreateUserAsync(
			User user);

		Task<Organization> GetUserAsync(
			string userName);

		Task<Organization> CreateOrganizationAsync(
			Organization org);

		Task AddUserToOrganizationAsync(
			Membership membership);

		Task<Organization> GetOrganizationAsync(
			string organizationName);

		Task<List<User>> GetUsersForOrganizationAsync(
			string organizationName);

		Task<List<Organization>> GetOrganizationsForUserAsync(
			string userName);

		Task<Organization> UpdatePaymentPlanForOrganization(
			string organizationName,
			PaymentPlan plan);

		Task<User> UpdatePaymentPlanForUser(
			string userName,
			PaymentPlan plan);
	}
}
