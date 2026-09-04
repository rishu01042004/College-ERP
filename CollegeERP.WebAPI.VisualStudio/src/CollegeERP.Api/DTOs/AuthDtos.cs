using System.ComponentModel.DataAnnotations;

namespace CollegeERP.Api.DTOs;

public sealed class LoginRequest
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
}

public sealed record AuthUserDto(Guid Id, string Name, string Email, string Role, string Department, string Avatar, string Color);
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAtUtc, AuthUserDto User);
public sealed record ChangePasswordRequest([property: Required] string CurrentPassword, [property: Required, MinLength(8)] string NewPassword);
