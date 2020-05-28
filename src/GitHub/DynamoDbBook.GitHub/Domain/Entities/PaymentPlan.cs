using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public class PaymentPlan
    {
		[JsonConstructor]
		private PaymentPlan(){}

		public PaymentPlan(
			PaymentPlanType planType,
			DateTime planStartDate)
		{
			this.PlanType = planType;
			this.PlanStartDate = planStartDate;
		}

		[JsonProperty]
        public PaymentPlanType PlanType { get; private set; }

        [JsonProperty]
        public DateTime PlanStartDate { get; private set; }
    }
}
