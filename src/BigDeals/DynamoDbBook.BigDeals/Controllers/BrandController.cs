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
    public class BrandController : ControllerBase
    {
		private readonly ILogger<BrandController> _logger;

		private readonly IBrandRepository _brandRepository;

		public BrandController(ILogger<BrandController> logger,
			IBrandRepository brandRepository)
		{
			_logger = logger;
			this._brandRepository = brandRepository;
		}

		[HttpPost("brands")]
		public async Task<Brand> CreateBrand(
			[FromBody] BrandDTO brand)
		{
			return await this._brandRepository.CreateAsync(
				Brand.Create(
					brand.Name,
					brand.LogoUrl));
		}

		[HttpGet("brands")]
		public async Task<IEnumerable<string>> ListBrands()
		{
			return await this._brandRepository.ListBrandsAsync();
		}

		[HttpGet("brands/{brandName}")]
		public async Task<Brand> GetBrand(string brandName)
		{
			return await this._brandRepository.GetBrandAsync(brandName);
		}

		[HttpPost("brands/{brandName}/likes/{username}")]
		public async Task<IActionResult> LikeBrand(string brandName, string username)
		{
			await this._brandRepository.LikeBrandAsync(
				brandName,
				username).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPost("brands/{brandName}/watches/{username}")]
		public async Task<IActionResult> WatchBrand(string brandName, string username)
		{
			await this._brandRepository.WatchBrandAsync(
				brandName,
				username).ConfigureAwait(false);

			return this.Ok();
		}
    }
}
