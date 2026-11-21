using System.ComponentModel.DataAnnotations;

namespace CarRental.Dtos;

public sealed record RegisterRequest
{
    [Required, MinLength(3), MaxLength(50)]
    public string Username { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(8), MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}

public sealed record LoginRequest
{
    [Required, MinLength(3), MaxLength(50)]
    public string Username { get; init; } = string.Empty;

    [Required, MinLength(1), MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}

public sealed record AuthResponse(string Token, DateTime ExpiresAtUtc, string Username, string Role);
