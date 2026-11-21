using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersController(ICustomerRepository customerRepository) => _customerRepository = customerRepository;

    [HttpGet]
    public ActionResult<IEnumerable<Customer>> Get() => Ok(_customerRepository.GetCustomers());

    [HttpGet("{id:int}")]
    public ActionResult<Customer> GetById(int id)
    {
        if (id <= 0) return BadRequest();
        var customer = _customerRepository.GetCustomerById(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<Customer> Post([FromBody] Customer customer)
    {
        if (!_customerRepository.InsertCustomer(customer)) return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = customer.IdCustomer }, customer);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public IActionResult Put([FromBody] Customer customer)
    {
        if (customer.IdCustomer <= 0) return BadRequest();
        return _customerRepository.UpdateCustomer(customer) ? NoContent() : NotFound();
    }
}
