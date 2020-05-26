using System;
using System.Collections.Generic;
using System.Net;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbBook.GitHub.LocalSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var dynamoDbConfig = new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" };

			var client = new AmazonDynamoDBClient(dynamoDbConfig);
			var tables = client.ListTablesAsync().Result;

			if (tables.TableNames.Contains("GitHub"))
			{
				client.DeleteTableAsync(new DeleteTableRequest("GitHub"));
			}

			Console.WriteLine("Creating table");

			var request = new CreateTableRequest
							  {
								  TableName = "GitHub",
								  AttributeDefinitions = new List<AttributeDefinition>(2)
															 {
																 new AttributeDefinition(
																	 "PK",
																	 ScalarAttributeType.S),
																 new AttributeDefinition(
																	 "SK",
																	 ScalarAttributeType.S),
																 new AttributeDefinition(
																	 "GSI1PK",
																	 ScalarAttributeType.S),
																 new AttributeDefinition(
																	 "GSI1SK",
																	 ScalarAttributeType.S),
															 },
								  KeySchema = new List<KeySchemaElement>(1)
												  {
													  new KeySchemaElement(
														  "PK",
														  KeyType.HASH),
													  new KeySchemaElement(
														  "SK",
														  KeyType.RANGE),
												  },

								  GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>(1)
															   {
																   new GlobalSecondaryIndex()
																	   {
																		   IndexName = "GSI1",
																		   KeySchema = new List<KeySchemaElement>(1)
																						   {
																							   new KeySchemaElement(
																								   "GSI1PK",
																								   KeyType.HASH),
																							   new KeySchemaElement(
																								   "GSI1SK",
																								   KeyType.RANGE),
																						   },
																		   Projection = new Projection()
																							{
																								ProjectionType = ProjectionType.ALL
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

			if (createResult.HttpStatusCode != HttpStatusCode.OK)
			{
				throw new Exception($"Failure creating table");
			}
        }
    }
}
