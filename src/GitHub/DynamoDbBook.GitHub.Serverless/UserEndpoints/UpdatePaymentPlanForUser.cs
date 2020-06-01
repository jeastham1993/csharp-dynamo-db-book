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
    public class UpdatePaymentPlanForUser
    {
	    private readonly IUserRepository _userRepository;

	    public UpdatePaymentPlanForUser()
	    {
		    var serviceCollection = new ServiceCollection().AddLogging().ConfigureDynamoDb();
		    var serviceProvider = serviceCollection.BuildServiceProvider();

		    this._userRepository = serviceProvider.GetRequiredService<IUserRepository>();
	    }

	    public async Task<APIGatewayProxyResponse> Execute(
		    APIGatewayProxyRequest request,
		    ILambdaContext context)
	    {
		    var paymentPlan = JsonConvert.DeserializeObject<UpdatePaymentPlanDTO>(request.Body);

		    await this._userRepository.UpdatePaymentPlanForUser(
			    HttpUtility.UrlDecode(request.PathParameters["username"]),
			    new PaymentPlan(paymentPlan.PlanType, DateTime.Now));

		    return new APIGatewayProxyResponse
		    {
			    StatusCode = 200,
		    };
	    }
    }
}
