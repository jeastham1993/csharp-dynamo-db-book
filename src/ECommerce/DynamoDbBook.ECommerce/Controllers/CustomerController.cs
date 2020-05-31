using System.Threading.Tasks;

using Amazon.DynamoDBv2.Model;

using DynamoDbBook.ECommerce.Domain.Entities;
using DynamoDbBook.ECommerce.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbBook.ECommerce.Controllers
{
    [ApiController]
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

		[HttpGet("customers/{username}")]
		public async Task<ActionResult<Customer>> GetCustomer(string username)
		{
			var customer = await this._customerRepo.GetCustomerAsync(username).ConfigureAwait(false);

			if (customer != null)
			{
				return this.Ok(customer);
			}
			else
			{
				return this.NotFound("Customer does not exist");
			}
		}

		[HttpPost("customers")]
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

		[HttpPost("customers/{username}/addresses")]
		public async Task<ActionResult<Customer>> AddAddress(
			string username,
			[FromBody] UpdateCustomerDTO customerUpdate)
		{
			try
			{
				await this._customerRepo.AddAddressAsync(
					username,
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

		[HttpDelete("customers/{username}/address/{name}")]
		public async Task<ActionResult<Customer>> DeleteAddress(string username, string name)
		{
			try
			{
				await this._customerRepo.DeleteAddressAsync(
					username,
					name);

				return this.Ok();
			}
			catch (ConditionalCheckFailedException ex)
			{
				return this.NotFound("Customer not found");
			}
		}
    }
}
