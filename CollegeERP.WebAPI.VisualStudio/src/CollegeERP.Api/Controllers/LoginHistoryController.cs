using CollegeERP.Api.Data; using CollegeERP.Api.DTOs; using CollegeERP.Api.Extensions;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CollegeERP.Api.Controllers;
[ApiController,Route("api/login-history"),Authorize(Roles="Admin,Principal")]
public sealed class LoginHistoryController(AppDbContext db):ControllerBase
{
 [HttpGet] public async Task<ActionResult<PagedResult<LoginHistoryDto>>> Get([FromQuery] EntityQueryParameters p,[FromQuery] DateTime? from,[FromQuery] DateTime? to,CancellationToken ct){var q=db.LoginHistory.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(p.Search))q=q.Where(x=>x.UserName.Contains(p.Search)||x.Email.Contains(p.Search)||x.IpAddress.Contains(p.Search)||x.Device.Contains(p.Search));if(!string.IsNullOrWhiteSpace(p.Status))q=q.Where(x=>x.Status==p.Status);if(from.HasValue)q=q.Where(x=>x.TimeUtc>=from.Value);if(to.HasValue)q=q.Where(x=>x.TimeUtc<=to.Value);return Ok(await q.OrderByDescending(x=>x.TimeUtc).Select(x=>new LoginHistoryDto(x.Id,x.UserName,x.Email,x.IpAddress,x.Device,x.TimeUtc,x.Status)).ToPagedResultAsync(p.Page,p.PageSize,ct));}
}
