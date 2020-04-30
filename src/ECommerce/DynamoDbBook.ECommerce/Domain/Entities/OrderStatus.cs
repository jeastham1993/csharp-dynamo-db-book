using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public enum OrderStatus
    {
        PLACED,
        PICKED,
        SHIPPED,
        DELIVERED,
        CANCELLED
    }
}
