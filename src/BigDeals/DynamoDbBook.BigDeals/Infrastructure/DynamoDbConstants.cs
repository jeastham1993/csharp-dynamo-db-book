using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.BigDeals.Infrastructure
{
    public static class DynamoDbConstants
	{
		public static string TableName = "BigDeals";

		public static int BrandShards = 5;
	}
}
