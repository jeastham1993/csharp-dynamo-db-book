using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public interface ICategoryRepository
    {
		Task<Category> CreateOrUpdateAsync(
			Category categoryToCreate);

		Task<Category> GetCategoryAsync(
			string categoryName);

		Task LikeCategoryAsync(
			Category category,
			string username);

		Task WatchCategoryAsync(
			Category category,
			string username);

		Task<IEnumerable<string>> FindWatchersForCategory(
			string categoryName);
    }
}
