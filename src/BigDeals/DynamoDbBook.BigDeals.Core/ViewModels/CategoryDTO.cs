using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.BigDeals.Domain.Entities;

namespace DynamoDbBook.BigDeals.ViewModels
{
    public class CategoryDTO
    {
        public string CategoryName { get; set; }

        public List<Deal> FeaturedDeals { get; set; } 
    }
}
