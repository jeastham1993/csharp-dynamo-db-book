# C# DynamoDB Book

Examples from the DynamoDB book by Alex DeBrie re-written in C#.

## Session Store
This example will show how to build a session store for an application that includes an authentication element. To model this out, we’ll use a simple primary key and a single secondary index in our DynamoDB table. Simple primary keys can be great for basic key-value access patterns where you’re only working with a single item at a time. However, you are limited in the amount of complexity you can handle with a simple primary key.

Because the concepts of simple primary keys are pretty straightforward, this is the only example that uses a simple primary key.

