using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.ViewModels
{
    public class CreateDealDTO
    {
		public string Title { get; set; }
		public string Link { get; set; }
		public decimal Price { get; set; }
		public string Category { get; set; }
		public string Brand { get; set; }
    }

}
