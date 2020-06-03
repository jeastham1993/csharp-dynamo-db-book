using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities
{
    public abstract class Entity
    {
		public Entity()
		{
			this.CreatedAt = DateTime.Now;
			this.UpdatedAt = DateTime.Now;
		}

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
    }
}
