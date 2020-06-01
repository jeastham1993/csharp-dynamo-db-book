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
    public class CategoryController : ControllerBase
    {
		private readonly ICategoryRepository _categoryRepository;

		public CategoryController(ICategoryRepository categoryRepository)
		{
			this._categoryRepository = categoryRepository;
		}

		[HttpPost("categories")]
		public async Task<Category> CreateCategory(
			[FromBody] CategoryDTO category)
		{
			return await this._categoryRepository.CreateOrUpdateAsync(
				Category.Create(
					category.CategoryName,
					category.FeaturedDeals));
		}

		[HttpPost("categories/{categoryName}")]
		public async Task<Category> UpdateCategory(
			string categoryName,
			[FromBody] CategoryDTO category)
		{
				return await this._categoryRepository.CreateOrUpdateAsync(
						   Category.Create(
							   category.CategoryName,
							   category.FeaturedDeals));
		}

		[HttpPost("categories/{categoryName}")]
		public async Task<Category> GetCategory(string categoryName)
		{
			return await this._categoryRepository.GetCategoryAsync(categoryName);
		}

		[HttpPost("categories/{categoryName}/likes/{username}like")]
		public async Task<IActionResult> LikeCategory(string categoryName, string username)
		{
			await this._categoryRepository.LikeCategoryAsync(
				categoryName,
				username).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPost("categories/{categoryName}/likes/{username}like")]
		public async Task<IActionResult> WatchCategory(string categoryName, string username)
		{
			await this._categoryRepository.WatchCategoryAsync(
				categoryName,
				username).ConfigureAwait(false);

			return this.Ok();
		}
    }
}
