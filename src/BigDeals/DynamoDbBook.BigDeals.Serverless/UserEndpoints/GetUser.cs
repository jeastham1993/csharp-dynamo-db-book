using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.BigDeals.Domain.Entities;
using DynamoDbBook.BigDeals.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.BigDeals.Serverless.UserEndpoints
{
    public class GetMessagesForUser
    {
	    private readonly IUserRepository _userRepository;

	    public GetMessagesForUser()
	    {
		    var serviceCollection = new ServiceCollection()
			    .AddLogging()
			    .ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._userRepository = serviceProvider.GetRequiredService<IUserRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var user = await this._userRepository.GetUserAsync(request.PathParameters["username"]).ConfigureAwait(false);

		    if (user != null)
		    {
			    return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(user)};
		    }
		    else
		    {
			    return new APIGatewayProxyResponse { StatusCode = 404, Body = "Deal not found"};
		    }
	    }
    }
}
