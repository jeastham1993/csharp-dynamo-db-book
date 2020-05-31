using DynamoDbBook.GitHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class UpdatePaymentPlanDTO
    {
        public PaymentPlanType PlanType {get;set;}
    }
}
