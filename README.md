# C# DynamoDB Book

Examples from the DynamoDB book by Alex DeBrie re-written in C#.

## Running the samples

There is a master solution file in the root of this folder, each example application also has it's own application specific solution file.

All of the samples include both a .NET core Web API and a serverless.yml file for running within AWS as Lambda functions and an AWS gateway.

### Serverless

To run the serverless samples, follow the practices detailed within the documentation on [serverless.com](https://www.serverless.com) to deploy the serverless.yml file to your AWS account.

### .NET Core Web API

To run local versions of the web API's, you'll first need to ensure you have Docker installed. Once Docker is installed, use the docker-compose file to start up a localstack instance

```bash
docker-compose up -d
```

Within each of the sample applications, there is a file named <application_name>.LocalSetup (e.g. DynamoDbBook.ECommerce.Setup)). Within the folder, there is a console application that creates the local DynamoDB tables. Run the console app using

```bash
dotnet run
```

Once the console application has completed, startup the API using the method of your choice (VS debugger, dotnet run etc.)

## Code layout

In all cases, DynamoDB items are created using extensions methods. This is to keep the core domain model clean, without adding the clutter of PK, SK and GSI's.

``` csharp

public static class SessionExtensions
{
    public static Dictionary<string, AttributeValue> AssAttributeMap(this Session session)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        var attributeMap = new Dictionary<string, AttributeValue>(5);

        attributeMap.Add("SessionId", new AttributeValue(session.SessionId.ToString()));
        attributeMap.Add("Username", new AttributeValue(session.Username));
        attributeMap.Add("CreatedAt", new AttributeValue(session.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        attributeMap.Add("ExpiresAt", new AttributeValue(session.ExpiresAt.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        attributeMap.Add("TTL", new AttributeValue()
                                    {
                                        N = ToEpoch(session.ExpiresAt).ToString()
                                    });

        return attributeMap;
    }

    public static long ToEpoch(DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
}


```

This also allows nice clean code in the repository

``` csharp

public async Task<Session> CreateSession(
			Session sessionToCreate)
{
    var putItemRequest = new PutItemRequest()
                                {
                                    TableName = DynamoDbConstants.TableName,
                                    Item = sessionToCreate.AssAttributeMap(),
                                    ConditionExpression = "attribute_not_exists(SessionId)"
                                };

    try
    {
        var result = await this._client.PutItemAsync(putItemRequest).ConfigureAwait(false);

        return sessionToCreate;
    }
    catch (ConditionalCheckFailedException ex)
    {
        this._logger.LogError("Holy moley -- a UUID collision!");

        return null;
    }
}

```

There are a couple of different methods for interacting with DynamoDB samples through the applications, the main difference being the way attributes are stored. 

In the session respository (example above) all attributes are stored a top level items.

However, in the Big Deals and GitHub samples attributes are all stored in a single 'Data' attribute.

``` csharp

public static Dictionary<string, AttributeValue> AsItem(this Repository repository)
{
    if (repository == null)
    {
        throw new ArgumentNullException(nameof(Repository));
    }

    var attributeMap = new Dictionary<string, AttributeValue>(5);

    foreach(var map in repository.AsKeys())
    {
        attributeMap.Add(map.Key, map.Value);	
    }

    foreach (var map in repository.AsGsi1())
    {
        attributeMap.Add(
            map.Key,
            map.Value);
    }

    foreach (var map in repository.AsGsi2())
    {
        attributeMap.Add(
            map.Key,
            map.Value);
    }

    foreach (var map in repository.AsGsi3())
    {
        attributeMap.Add(
            map.Key,
            map.Value);
    }

    attributeMap.Add("Type", new AttributeValue("Repository"));
    attributeMap.Add(
        "Data",
        new AttributeValue(){ M = repository.AsData()});

    return attributeMap;
}

public static Dictionary<string, AttributeValue> AsData(
    this Repository repository)
{
    var document = Document.FromJson(JsonConvert.SerializeObject(repository));
    var documentAttributeMap = document.ToAttributeMap();

    return documentAttributeMap;
}

```

This method was taken from a 2020 session with Rick Houlihan in which he indicated that for high performance application, a single top level map attribute can give a performance benefit.

There is a danger here that a set of attributes will break the 400kb limit of a map attribute, and this would need to be considered in any production application.

At a top level, all the keys (PK, SK, GSI's) are included as well as a Type attribute to indicate what the data attribute contains.

