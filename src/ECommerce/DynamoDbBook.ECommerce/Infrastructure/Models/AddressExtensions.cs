using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;

namespace DynamoDbBook.ECommerce.Infrastructure.Models
{
    public static class AddressExtensions
    {
		public static Dictionary<string, AttributeValue> AsAttributeMap(
			this Address address)
		{
			var attributeMap = new Dictionary<string, AttributeValue>(3);
			attributeMap.Add(
				"StreetAddress",
				new AttributeValue(address.StreetAddress));

			attributeMap.Add(
				"PostcalCode",
				new AttributeValue(address.PostalCode));

			attributeMap.Add(
				"Country",
				new AttributeValue(address.Country));

			return attributeMap;
		}
    }
}
