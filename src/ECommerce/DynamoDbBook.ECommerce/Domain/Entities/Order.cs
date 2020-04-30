using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using DynamoDbBook.SharedKernel;

namespace DynamoDbBook.ECommerce.Domain.Entities
{
    public class Order
    {
		private Order()
		{
			this.OrderId = Ksuid.Generate().ToString();
			this.CreatedAt = DateTime.Now;
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
						   Username = username
					   };
		}

		private List<OrderItem> _items;

		public string Username { get; private set; }

		public string OrderId { get; private set; }

        public Address Address { get; set; }

		public DateTime CreatedAt { get; private set; }

		public OrderStatus Status { get; set; }

		public decimal TotalAmount => this._items.Sum(p => p.Amount);

		public int NumberItems => this._items.Count;

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
	}
}
