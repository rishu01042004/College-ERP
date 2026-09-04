using CollegeERP.Api.Data;
using CollegeERP.Api.DTOs;
using CollegeERP.Api.Models;
using CollegeERP.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AppDbContext db, PasswordService passwords, JwtTokenService tokens, CurrentUserService currentUser) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.Include(x => x.Role).Include(x => x.Department)
            .FirstOrDefaultAsync(x => x.Email.ToLower() == email, ct);

        if (user is null || !passwords.Verify(request.Password, user.PasswordHash) || !user.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
        {
            db.LoginHistory.Add(NewHistory(user, request.Email, "Failed"));
            await db.SaveChangesAsync(ct);
            return Unauthorized(new ApiMessage("Invalid email or password."));
        }

        user.LastLoginUtc = DateTime.UtcNow;
        db.LoginHistory.Add(NewHistory(user, user.Email, "Success"));
        await db.SaveChangesAsync(ct);

        var token = tokens.Create(user);
        return Ok(new LoginResponse(token.Token, token.ExpiresAtUtc,
            new AuthUserDto(user.Id, user.Name, user.Email, user.Role.Name, user.Department?.Code ?? "", user.Avatar, user.Color)));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<AuthUserDto>> Me(CancellationToken ct)
    {
        if (currentUser.UserId is not Guid id) return Unauthorized();
        var user = await db.Users.AsNoTracking().Include(x => x.Role).Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id, ct);
        return user is null ? NotFound() : Ok(new AuthUserDto(user.Id, user.Name, user.Email, user.Role.Name, user.Department?.Code ?? "", user.Avatar, user.Color));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiMessage>> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        if (currentUser.UserId is not Guid id) return Unauthorized();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (user is null) return NotFound(new ApiMessage("User not found."));
        if (!passwords.Verify(request.CurrentPassword, user.PasswordHash)) return BadRequest(new ApiMessage("Current password is incorrect."));
        user.PasswordHash = passwords.Hash(request.NewPassword);
        await db.SaveChangesAsync(ct);
        return Ok(new ApiMessage("Password changed successfully."));
    }

    private LoginHistory NewHistory(UserAccount? user, string email, string status) => new()
    {
        User = user,
        UserName = user?.Name ?? "Unknown",
        Email = email.Trim(),
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
        Device = Request.Headers.UserAgent.ToString(),
        TimeUtc = DateTime.UtcNow,
        Status = status
    };
}
