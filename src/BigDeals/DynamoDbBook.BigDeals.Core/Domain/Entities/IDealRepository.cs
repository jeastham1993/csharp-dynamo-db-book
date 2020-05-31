using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public interface IDealRepository
	{
		Task<Deal> CreateAsync(
			Deal dealToCreate);

		Task<IEnumerable<Deal>> FetchDealsForDateAsync(
			DateTime date,
			string lastSeen = "$",
			int limit = 25);

		Task<IEnumerable<Deal>> FetchDealsForBrandAndDateAsync(
			string brand,
			DateTime date,
			string lastSeen = "$",
			int limit = 25);

		Task<IEnumerable<Deal>> FetchDealsForCategoryAndDateAsync(
			string category,
			DateTime date,
			string lastSeen = "$",
			int limit = 25);

		Task<Deal> GetDealAsync(
			string dealId);

		Task UpdateEditorsChoiceAsync(
			IEnumerable<Deal> deals);

		Task UpdateFrontPageAsync(
			IEnumerable<Deal> deals);

		Task<IEnumerable<Deal>> GetEditorsChoiceAsync();

		Task<IEnumerable<Deal>> GetFrontPageAsync();
	}
}
