using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CollegeERP.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace CollegeERP.Api.Services;

public sealed record TokenResult(string Token, DateTime ExpiresAtUtc);

public sealed class JwtTokenService(IConfiguration configuration)
{
    public TokenResult Create(UserAccount user)
    {
        var section = configuration.GetSection("Jwt");
        var issuer = section["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
        var audience = section["Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
        var key = section["Key"] ?? throw new InvalidOperationException("Jwt:Key missing");
        var expiryMinutes = int.TryParse(section["ExpiryMinutes"], out var minutes) ? minutes : 480;
        var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.Name),
            new("department", user.Department?.Code ?? string.Empty),
            new("studentId", user.StudentId?.ToString() ?? string.Empty),
            new("facultyId", user.FacultyId?.ToString() ?? string.Empty)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(issuer, audience, claims, DateTime.UtcNow, expires, credentials);
        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
