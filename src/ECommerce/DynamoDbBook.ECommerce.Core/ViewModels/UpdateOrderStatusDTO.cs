using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DynamoDbBook.ECommerce.Domain.Entities;

namespace DynamoDbBook.ECommerce.ViewModels
{
    public class UpdateOrderStatusDTO
    {
        public OrderStatus NewStatus { get; set; }
    }
}
