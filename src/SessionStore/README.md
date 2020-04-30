# Session Store
This example will show how to build a session store for an application that includes an authentication element. To model this out, we’ll use a simple primary key and a single secondary index in our DynamoDB table. Simple primary keys can be great for basic key-value access patterns where you’re only working with a single item at a time. However, you are limited in the amount of complexity you can handle with a simple primary key.

Because the concepts of simple primary keys are pretty straightforward, this is the only example that uses a simple primary key.

## Usage

First, startup the localstack docker image using the docker-compose file in the repository root

``` bash
docker-compose up -d
```

Then run the console application in the DynamoDbBook.SessionStore.Setup folder. This console application handles the creation of the DynamoDB table and is hardcoded to use the localstack implementation. If a different URL is required, or you'd like to create in DynamoDB within AWS the code will need to be changed.

``` bash
cd .\src\SessionStore\DynamoDbBook.SessionStore.Setup\
dotnet run
```

Then start a new debug session using the API in DynamoDbBook.SessionStore folder. The API includes Swagger UI for testing the API.