using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Core.Domain.Request;
using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DynamoDbBook.BigDeals.Controllers
{
	[ApiController]
    public class DealController : ControllerBase
    {
		private readonly ILogger<DealController> _logger;

		private readonly IDealRepository _dealRepository;

		private readonly SendHotDealInteractor _hotDealInteractor;

		public DealController(ILogger<DealController> logger,
			IDealRepository dealRepository,
			SendHotDealInteractor hotDealInteractor)
		{
			_logger = logger;
			this._dealRepository = dealRepository;
			this._hotDealInteractor = hotDealInteractor;
		}

		[HttpPost("deals")]
		public async Task<Deal> CreateDeal(
			CreateDealDTO newDeal)
		{
			var deal = Deal.Create(newDeal.Title);
			deal.Price = newDeal.Price;
			deal.Brand = newDeal.Brand;
			deal.Category = newDeal.Category;
			deal.Link = newDeal.Link;

			return await this._dealRepository.CreateAsync(deal)
							   .ConfigureAwait(false);
		}

		[HttpPost("hotdealblast")]
		public async Task<IActionResult> HotDealBlast(
			[FromBody] SendHotDealRequest request)
		{
			await this._hotDealInteractor.Handle(request).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpGet("deals")]
		public async Task<IEnumerable<Deal>> GetForDate(
			DateTime date,
			string lastSeen = "",
			int limit = 25
			)
		{
			return await this._dealRepository.FetchDealsForDateAsync(date, lastSeen, limit)
					   .ConfigureAwait(false);
		}

		[HttpGet("deals/brand/{brandName}")]
		public async Task<IEnumerable<Deal>> GetForBrandAndDate(
			string brandName,
			DateTime date,
			string lastSeen = "",
			int limit = 25
		)
		{
			return await this._dealRepository.FetchDealsForBrandAndDateAsync(brandName, date, lastSeen, limit)
					   .ConfigureAwait(false);
		}

		[HttpGet("deals/brand/{brandName}")]
		public async Task<IEnumerable<Deal>> GetForCategoryAndDate(
			string categoryName,
			DateTime date,
			string lastSeen = "",
			int limit = 25
		)
		{
			return await this._dealRepository.FetchDealsForCategoryAndDateAsync(categoryName, date, lastSeen, limit)
					   .ConfigureAwait(false);
		}

		[HttpPut("editorschoice")]
		public async Task<IActionResult> UpdateEditorsChoice(
			[FromBody] List<Deal> deals)
		{
			await this._dealRepository.UpdateEditorsChoiceAsync(deals).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpPut("frontpage")]
		public async Task<IActionResult> UpdateFrontPage(
			[FromBody] List<Deal> deals)
		{
			await this._dealRepository.UpdateFrontPageAsync(deals).ConfigureAwait(false);

			return this.Ok();
		}

		[HttpGet("editorschoice")]
		public async Task<IEnumerable<Deal>> GetEditorsChoice()
		{
			return await this._dealRepository.GetEditorsChoiceAsync().ConfigureAwait(false);
		}

		[HttpGet("frontpage")]
		public async Task<IEnumerable<Deal>> GetFrontPage()
		{
			return await this._dealRepository.GetFrontPageAsync().ConfigureAwait(false);
		}
    }
}
