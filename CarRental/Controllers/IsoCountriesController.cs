using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public sealed class IsoCountriesController : ControllerBase
{
    private readonly ICountryRepository _countryRepository;

    public IsoCountriesController(ICountryRepository countryRepository) => _countryRepository = countryRepository;

    [HttpGet]
    public ActionResult<IEnumerable<Country>> Get() => Ok(_countryRepository.GetCountries());

    [HttpGet("{id:int}")]
    public ActionResult<Country> GetById(int id)
    {
        if (id <= 0) return BadRequest();
        var country = _countryRepository.GetCountryById(id);
        return country is null ? NotFound() : Ok(country);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<Country> Post([FromBody] Country country)
    {
        if (!_countryRepository.InsertCountry(country)) return BadRequest();
        return CreatedAtAction(nameof(GetById), new { id = country.CountryId }, country);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public IActionResult Put([FromBody] Country country)
    {
        if (country.CountryId <= 0) return BadRequest();
        return _countryRepository.UpdateCountry(country) ? NoContent() : NotFound();
    }
}
