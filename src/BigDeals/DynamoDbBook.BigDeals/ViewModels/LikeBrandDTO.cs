using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;

namespace DynamoDbBook.BigDeals.ViewModels
{
    public class LikeBrandDTO
    {
        public Brand Brand { get; set; }

		public string Username { get; set; }
    }
}
