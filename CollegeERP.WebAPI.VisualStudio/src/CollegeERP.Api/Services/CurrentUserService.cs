using System.Security.Claims;

namespace CollegeERP.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor accessor)
{
    private ClaimsPrincipal User => accessor.HttpContext?.User ?? new ClaimsPrincipal();
    public Guid? UserId => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string Name => User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
    public string Email => User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
    public string Role => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    public string Department => User.FindFirstValue("department") ?? string.Empty;
    public Guid? StudentId => Guid.TryParse(User.FindFirstValue("studentId"), out var id) ? id : null;
    public Guid? FacultyId => Guid.TryParse(User.FindFirstValue("facultyId"), out var id) ? id : null;
    public bool IsInRole(params string[] roles) => roles.Any(r => string.Equals(r, Role, StringComparison.OrdinalIgnoreCase));
}
