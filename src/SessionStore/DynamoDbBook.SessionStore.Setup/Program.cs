using System;
using System.Collections.Generic;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbBook.SessionStore.Setup
{
	class Program
	{
		static void Main(
			string[] args)
		{
			var dynamoDbConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" };

			var client = new AmazonDynamoDBClient(dynamoDbConfig);
			var tables = client.ListTablesAsync().Result;

			if (tables.TableNames.Contains("SessionStore"))
			{
				client.DeleteTableAsync(new DeleteTableRequest("SessionStore"));
			}

			Console.WriteLine("Creating table");

			var request = new CreateTableRequest
							  {
								  TableName = "SessionStore",
								  AttributeDefinitions = new List<AttributeDefinition>(2)
															 {
																 new AttributeDefinition(
																	 "SessionToken",
																	 ScalarAttributeType.S),
																 new AttributeDefinition(
																	 "Username",
																	 ScalarAttributeType.S),
															 },
								  KeySchema = new List<KeySchemaElement>(1)
												  {
													  new KeySchemaElement(
														  "SessionToken",
														  KeyType.HASH),
												  },
								  GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>(1)
															   {
																   new GlobalSecondaryIndex()
																	   {
																		   IndexName = "UserIndex",
																		   KeySchema = new List<KeySchemaElement>(1)
																						   {
																							   new KeySchemaElement(
																								   "Username",
																								   KeyType.HASH),
																						   },
																		   Projection = new Projection()
																							{
																								ProjectionType = ProjectionType.KEYS_ONLY
																							},
																		   ProvisionedThroughput = new ProvisionedThroughput(
																			   10,
																			   10),
																	   }
															   },
								  ProvisionedThroughput = new ProvisionedThroughput(
									  10,
									  10),
							  };

			var createResult = client.CreateTableAsync(request).Result;
		}
	}
}