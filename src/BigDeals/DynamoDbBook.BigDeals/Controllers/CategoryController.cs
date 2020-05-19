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
	[Route("[controller]")]
    public class CategoryController : ControllerBase
    {
		private readonly ILogger<CategoryController> _logger;

		private readonly ICategoryRepository _categoryRepository;

		public CategoryController(ILogger<CategoryController> logger,
			ICategoryRepository categoryRepository)
		{
			_logger = logger;
			this._categoryRepository = categoryRepository;
		}

		[HttpPost]
		public async Task<Category> CreateCategory(
			[FromBody] CategoryDTO category)
		{
			return await this._categoryRepository.CreateOrUpdateAsync(
				Category.Create(
					category.CategoryName,
					category.FeaturedDeals));
		}

		[HttpPut]
		public async Task<Category> UpdateCategory(
			[FromBody] CategoryDTO category)
		{
			return await this._categoryRepository.CreateOrUpdateAsync(
					   Category.Create(
						   category.CategoryName,
						   category.FeaturedDeals));
		}

		[HttpGet("{categoryName}")]
		public async Task<Category> GetCategory(string categoryName)
		{
			return await this._categoryRepository.GetCategoryAsync(categoryName);
		}

		[HttpPut("like")]
		public async Task<IActionResult> LikeCategory([FromBody] LikeCategoryDTO likeCategory)
		{
			await this._categoryRepository.LikeCategoryAsync(
				likeCategory.Category,
				likeCategory.Username).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPut("watch")]
		public async Task<IActionResult> WatchCategory([FromBody] LikeCategoryDTO likeCategory)
		{
			await this._categoryRepository.WatchCategoryAsync(
				likeCategory.Category,
				likeCategory.Username).ConfigureAwait(false);

			return this.Ok();
		}
    }
}
