using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers;

/// <summary>
/// Small, in-memory example used by the JWT practice to demonstrate public,
/// authenticated, and role-protected endpoints without depending on the car catalog database.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class VehiculosController : ControllerBase
{
    private static readonly string[] Vehicles = ["Toyota Corolla", "Honda Civic", "Ford Mustang"];

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<string>> GetAll() => Ok(Vehicles);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Create([FromBody] string modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo))
            return BadRequest(new { message = "El modelo es obligatorio." });

        return Created("", new { message = $"Vehículo '{modelo}' registrado correctamente." });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
            return BadRequest(new { message = "El identificador debe ser mayor que cero." });

        return Ok(new { message = $"Vehículo {id} eliminado por un Administrador." });
    }
}
