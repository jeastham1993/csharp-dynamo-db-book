using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.SessionStore.Domain.Exceptions
{
    public class InvalidSessionException : ArgumentException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidSessionException"/> class.
		/// </summary>
		public InvalidSessionException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidSessionException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		public InvalidSessionException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidSessionException"/> class.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="innerException">An inner exception.</param>
		public InvalidSessionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
