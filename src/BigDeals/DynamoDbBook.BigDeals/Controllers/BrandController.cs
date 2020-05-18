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

		[HttpPost]
		public async Task<Brand> CreateBrand(
			[FromBody] BrandDTO brand)
		{
			return await this._brandRepository.CreateAsync(
				Brand.Create(
					brand.Name,
					brand.LogoUrl));
		}

		[HttpGet]
		public async Task<IEnumerable<string>> ListBrands()
		{
			return await this._brandRepository.ListBrandsAsync();
		}
    }
}
