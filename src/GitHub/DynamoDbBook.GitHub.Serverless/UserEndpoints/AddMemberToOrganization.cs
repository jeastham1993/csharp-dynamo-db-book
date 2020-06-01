using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using DynamoDbBook.GitHub.Domain.Entities;
using DynamoDbBook.GithUb.Serverless;
using DynamoDbBook.GitHub.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Serverless.UserEndpoints
{
    public class AddMemberToOrganization
    {
	    private readonly IUserRepository _userRepository;

	    public AddMemberToOrganization()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._userRepository = serviceProvider.GetRequiredService<IUserRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var member = JsonConvert.DeserializeObject<AddMemberToOrgDTO>(request.Body);

		    await this._userRepository.AddUserToOrganizationAsync(
			    new Membership()
			    {
				    MemberSince = DateTime.Now,
				    OrganizationName = HttpUtility.UrlDecode(request.PathParameters["organizationName"]),
				    Role = member.Role,
				    Username = member.Username
			    });

		    return new APIGatewayProxyResponse
		    {
			    StatusCode = 200
		    };
	    }
    }
}
