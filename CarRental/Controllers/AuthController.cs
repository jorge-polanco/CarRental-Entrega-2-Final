using CarRental.Data;
using CarRental.Dtos;
using CarRental.Models;
using CarRental.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public sealed class AuthController(
    AppDbContext dbContext,
    IPasswordHasher<AppUser> passwordHasher,
    JwtTokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var username = request.Username.Trim();
        var normalizedUsername = username.ToUpperInvariant();
        var email = request.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(x => x.NormalizedUsername == normalizedUsername, cancellationToken))
            return Conflict(new { message = "El nombre de usuario ya está registrado." });

        if (await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
            return Conflict(new { message = "El correo ya está registrado." });

        var user = new AppUser
        {
            Username = username,
            NormalizedUsername = normalizedUsername,
            Email = email,
            Role = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        dbContext.Users.Add(user);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is MySqlConnector.MySqlException { Number: 1062 })
        {
            return Conflict(new { message = "El usuario o correo ya está registrado." });
        }

        return Created("", new { user.Id, user.Username, user.Email, user.Role });
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.Trim().ToUpperInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(
            x => x.NormalizedUsername == normalizedUsername,
            cancellationToken);

        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = "Credenciales incorrectas." });

        var token = tokenService.CreateToken(user);
        return Ok(new AuthResponse(token.Token, token.ExpiresAtUtc, user.Username, user.Role));
    }
}
