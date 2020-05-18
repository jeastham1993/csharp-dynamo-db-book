﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace DynamoDbBook.BigDeals.Controllers
{
	[ApiController]
	[Route("[controller]")]
    public class DealController : ControllerBase
    {
		private readonly ILogger<DealController> _logger;

		private readonly IDealRepository _dealRepository;

		public DealController(ILogger<DealController> logger,
			IDealRepository dealRepository)
		{
			_logger = logger;
			this._dealRepository = dealRepository;
		}

		[HttpPost]
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

		[HttpGet]
		public async Task<IEnumerable<Deal>> GetForDate(
			DateTime date,
			string lastSeen = "",
			int limit = 25
			)
		{
			return await this._dealRepository.FetchDealsForDateAsync(date, lastSeen, limit)
					   .ConfigureAwait(false);
		}

		[HttpGet("brand/{brandName}")]
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

		[HttpGet("category/{categoryName}")]
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
    }
}
