using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Domain.Entities
{
    public interface IBrandRepository
	{
		Task<Brand> CreateAsync(
			Brand brandToCreate);

		Task<IEnumerable<string>> ListBrandsAsync();

		Task<Brand> GetBrandAsync(
			string brandName);

		Task LikeBrandAsync(
			string brandName,
			string username);

		Task WatchBrandAsync(
			string brandName,
			string username);

		Task<IEnumerable<string>> FindWatchersForBrand(
			string brandName);
	}
}
