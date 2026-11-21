using CarRental.Controllers;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRental.Tests.Customers;

public sealed class CustomersControllerTests
{
    private readonly Mock<ICustomerRepository> _repository = new();

    [Fact]
    public void Get_ReturnsCustomersFromRepository()
    {
        var customers = new[] { CreateCustomer(1) };
        _repository.Setup(x => x.GetCustomers()).Returns(customers);
        var controller = new CustomersController(_repository.Object);

        var result = controller.Get();

        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(customers, response.Value);
        _repository.Verify(x => x.GetCustomers(), Times.Once);
    }

    [Fact]
    public void GetById_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.GetCustomerById(42)).Returns((Customer?)null);
        var controller = new CustomersController(_repository.Object);

        var result = controller.GetById(42);

        Assert.IsType<NotFoundResult>(result.Result);
        _repository.Verify(x => x.GetCustomerById(42), Times.Once);
    }

    [Fact]
    public void Post_InsertsCustomerAndReturnsCreated()
    {
        var customer = CreateCustomer(0);
        _repository.Setup(x => x.InsertCustomer(customer)).Returns(true);
        var controller = new CustomersController(_repository.Object);

        var result = controller.Post(customer);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, response.StatusCode ?? 201);
        Assert.Same(customer, response.Value);
        _repository.Verify(x => x.InsertCustomer(customer), Times.Once);
    }

    [Fact]
    public void Put_WhenRepositoryUpdatesCustomer_ReturnsOk()
    {
        var customer = CreateCustomer(42);
        _repository.Setup(x => x.UpdateCustomer(customer)).Returns(true);
        var controller = new CustomersController(_repository.Object);

        var result = controller.Put(customer);

        Assert.IsType<NoContentResult>(result);
        _repository.Verify(x => x.UpdateCustomer(customer), Times.Once);
    }

    private static Customer CreateCustomer(int id) => new()
    {
        IdCustomer = id,
        FirstName = "Jorge",
        LastName = "Ramírez",
        SocialSecurityNumber = "ABC123",
        Email = "jorge@example.test",
        CountryName = "México",
        CountryId = 1,
        MobilePhone = "5550101",
        StateName = "Quintana Roo"
    };
}
