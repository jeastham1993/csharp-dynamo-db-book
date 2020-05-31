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
    public class CreateUser
    {
	    private readonly IUserRepository _userRepository;

	    public CreateUser()
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
		    var userDto = JsonConvert.DeserializeObject<UserDTO>(request.Body);

		    var createdUser =
			    await this._userRepository.CreateAsync(User.Create(userDto.Username)).ConfigureAwait(false);

		    if (createdUser != null)
		    {
			    return new APIGatewayProxyResponse { StatusCode = 200, Body = JsonConvert.SerializeObject(createdUser)};
		    }
		    else
		    {
			    return new APIGatewayProxyResponse { StatusCode = 400, Body = "Order exists"};
		    }
	    }
    }
}
