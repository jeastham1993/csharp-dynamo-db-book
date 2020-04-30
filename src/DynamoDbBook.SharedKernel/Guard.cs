using System;

namespace DynamoDbBook.SharedKernel
{
    public static class Guard
    {
		public static object AgainstNull(object? input, string parameterName)
		{
			if (null == input)
			{
				throw new ArgumentNullException(parameterName);
			}

			return input;
		}

		public static string AgainstNullOrEmpty(string? input, string parameterName)
		{
			Guard.AgainstNull(input, parameterName);

			if (input == String.Empty)
			{
				throw new ArgumentException($"Required input {parameterName} was empty.", parameterName);
			}

			return input;
		}
    }
}
