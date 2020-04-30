using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.Infrastructure.Models;

using Newtonsoft.Json;

namespace DynamoDbBook.SessionStore.Infrastructure
{
	public static class CustomerExtensions
	{
		public static Dictionary<string, AttributeValue> AsKeys(this Customer customer)
		{
			if (customer == null)
			{
				throw new ArgumentNullException(nameof(customer));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"CUSTOMER#{customer.Username}"));
			attributeMap.Add("SK", new AttributeValue($"CUSTOMER#{customer.Username}"));

			return attributeMap;
		}
		
		public static Dictionary<string, AttributeValue> AsItem(this Customer customer)
		{
			if (customer == null)
			{
				throw new ArgumentNullException(nameof(customer));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in customer.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			var addressData = new Dictionary<string, AttributeValue>();

			foreach (var address in customer.Addresses)
			{
				addressData.Add(
					address.Name,
					new AttributeValue() { M = address.AsAttributeMap(), });
			}

			attributeMap.Add("Type", new AttributeValue("Customer"));
			attributeMap.Add("Username", new AttributeValue(customer.Username));
			attributeMap.Add("Email", new AttributeValue(customer.Email));
			attributeMap.Add("Name", new AttributeValue(customer.Name));
			attributeMap.Add("Addresses", new AttributeValue()
											  {
												  M = addressData
											  });
			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsCustomerEmailKeys(this Customer customer)
		{
			if (customer == null)
			{
				throw new ArgumentNullException(nameof(customer));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue($"CUSTOMEREMAIL#{customer.Email}"));
			attributeMap.Add("SK", new AttributeValue($"CUSTOMEREMAIL#{customer.Email}"));

			return attributeMap;
		}
		
		public static Dictionary<string, AttributeValue> AsCustomerEmailItem(this Customer customer)
		{
			if (customer == null)
			{
				throw new ArgumentNullException(nameof(customer));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in customer.AsCustomerEmailKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			attributeMap.Add("Type", new AttributeValue("CustomerEmail"));
			attributeMap.Add("Username", new AttributeValue(customer.Username));
			attributeMap.Add("Email", new AttributeValue(customer.Email));

			return attributeMap;
		}
	}
}