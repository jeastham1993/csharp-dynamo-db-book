using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

using Newtonsoft.Json;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public class Order
    {
		[JsonConstructor]
		internal Order()
		{
			this._items = new List<OrderItem>();
		}

		public static Order Create(
			string username,
			Address address)
		{
			Guard.AgainstNullOrEmpty(username, nameof(username));

			return new Order()
					   {
						   Address = address,
						   Username = username,
						   OrderId = Ksuid.Generate().ToString().Replace("/", string.Empty).Replace("=", string.Empty),
						   CreatedAt = DateTime.Now,
						   _items = new List<OrderItem>()
					   };
		}

		[JsonProperty("items")]
		private List<OrderItem> _items;

		[JsonProperty("username")]
		public string Username { get; internal set; }

		[JsonProperty]
		public string OrderId { get; internal set; }

		[JsonProperty]
		public string AddressString { get; private set; }

		[JsonProperty]
        public Address Address { get; set; }

		[JsonProperty]
		public DateTime CreatedAt { get; private set; }

		[JsonProperty]
		public OrderStatus Status { get; set; }

		[JsonProperty]
		public decimal TotalAmount
		{
			get
			{
				if (this._items == null)
				{
					return 0;
				}

				return this._items.Sum(p => p.TotalCost);
			}
		}

		[JsonProperty]
		public decimal NumberItems
		{
			get
			{
				if (this._items == null)
				{
					return 0;
				}

				return this._items.Count();
			}
		}

		[JsonIgnore]
		public IReadOnlyCollection<OrderItem> Items => this._items;

		public OrderItem AddItem(string description, decimal amount, decimal price)
		{
			var newItem = new OrderItem()
							  {
								  Amount = amount, Description = description, OrderId = this.OrderId, Price = price
							  };
			this._items.Add(
				newItem);

			return newItem;
		}

		public OrderItem AddItem(OrderItem item)
		{
			this._items.Add(
				item);

			return item;
		}
	}
}
