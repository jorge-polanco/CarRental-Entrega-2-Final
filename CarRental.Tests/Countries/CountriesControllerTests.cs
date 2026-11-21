using CarRental.Controllers;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CarRental.Tests.Countries;

public sealed class CountriesControllerTests
{
    private readonly Mock<ICountryRepository> _repository = new();

    [Fact]
    public void Get_ReturnsCountriesFromRepository()
    {
        var countries = new[] { CreateCountry(1) };
        _repository.Setup(x => x.GetCountries()).Returns(countries);
        var controller = new IsoCountriesController(_repository.Object);

        var result = controller.Get();

        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(countries, response.Value);
        _repository.Verify(x => x.GetCountries(), Times.Once);
    }

    [Fact]
    public void GetById_WhenCountryDoesNotExist_ReturnsNotFound()
    {
        _repository.Setup(x => x.GetCountryById(42)).Returns((Country?)null);
        var controller = new IsoCountriesController(_repository.Object);

        var result = controller.GetById(42);

        Assert.IsType<NotFoundResult>(result.Result);
        _repository.Verify(x => x.GetCountryById(42), Times.Once);
    }

    [Fact]
    public void Post_InsertsCountryAndReturnsCreated()
    {
        var country = CreateCountry(0);
        _repository.Setup(x => x.InsertCountry(country)).Returns(true);
        var controller = new IsoCountriesController(_repository.Object);

        var result = controller.Post(country);

        var response = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, response.StatusCode ?? 201);
        Assert.Same(country, response.Value);
        _repository.Verify(x => x.InsertCountry(country), Times.Once);
    }

    [Fact]
    public void Put_WhenRepositoryUpdatesCountry_ReturnsNoContent()
    {
        var country = CreateCountry(42);
        _repository.Setup(x => x.UpdateCountry(country)).Returns(true);
        var controller = new IsoCountriesController(_repository.Object);

        var result = controller.Put(country);

        Assert.IsType<NoContentResult>(result);
        _repository.Verify(x => x.UpdateCountry(country), Times.Once);
    }

    private static Country CreateCountry(int id) => new()
    {
        CountryId = id,
        Iso2Code = "MX",
        Iso3Code = "MEX",
        CountryName = "México"
    };
}
