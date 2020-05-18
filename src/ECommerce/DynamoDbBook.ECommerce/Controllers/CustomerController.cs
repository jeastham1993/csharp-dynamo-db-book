using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.ECommerce.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;

		private readonly ICustomerRepository _customerRepo;

		public CustomerController(
			ILogger<CustomerController> logger,
			ICustomerRepository customerRepo)
		{
			_logger = logger;
			this._customerRepo = customerRepo;
		}

		[HttpGet]
		public async Task<ActionResult<Customer>> GetCustomer(string userName)
		{
			var customer = await this._customerRepo.GetCustomerAsync(userName).ConfigureAwait(false);

			if (customer != null)
			{
				return this.Ok(customer);
			}
			else
			{
				return this.NotFound("Customer does not exist");
			}
		}

		[HttpPost]
		public async Task<ActionResult<Customer>> CreateCustomer(
			[FromBody] CreateCustomerDTO createCustomer)
		{
			var newCustomer = Customer.Create(
				createCustomer.Username,
				createCustomer.Email,
				createCustomer.Name);

			createCustomer.Addresses.ForEach(
				a => newCustomer.AddAddress(
					a.Name,
					a.StreetAddress,
					a.PostalCode,
					a.Country));

			var createdCustomer = await this._customerRepo.CreateAsync(newCustomer).ConfigureAwait(false);

			if (createdCustomer != null)
			{
				return this.Ok(createdCustomer);
			}
			else
			{
				return this.BadRequest("Customer exists");
			}
		}

		[HttpPut]
		public async Task<ActionResult<Customer>> AddAddress(
			[FromBody] UpdateCustomerDTO customerUpdate)
		{
			try
			{
				await this._customerRepo.AddAddressAsync(
					customerUpdate.Username,
					new Address(
						customerUpdate.Address.Name,
						customerUpdate.Address.StreetAddress,
						customerUpdate.Address.PostalCode,
						customerUpdate.Address.Country));

				return this.Ok(customerUpdate);
			}
			catch (ConditionalCheckFailedException ex)
			{
				return this.NotFound("Customer not found");
			}
		}

		[HttpDelete("{username}/address")]
		public async Task<ActionResult<Customer>> DeleteAddress(string username, string addressName)
		{
			try
			{
				await this._customerRepo.DeleteAddressAsync(
					username,
					addressName);

				return this.Ok();
			}
			catch (ConditionalCheckFailedException ex)
			{
				return this.NotFound("Customer not found");
			}
		}
    }
}
