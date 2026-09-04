using CollegeERP.Api.Data; using CollegeERP.Api.DTOs; using CollegeERP.Api.Extensions; using CollegeERP.Api.Models;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CollegeERP.Api.Controllers;
[ApiController,Route("api/roles"),Authorize(Roles="Admin")]
public sealed class RolesController(AppDbContext db):ControllerBase
{
 [HttpGet] public async Task<ActionResult<PagedResult<RoleDto>>> Get([FromQuery] EntityQueryParameters p,CancellationToken ct){var q=db.Roles.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(p.Search))q=q.Where(x=>x.Name.Contains(p.Search)||x.Description.Contains(p.Search)||x.Permissions.Contains(p.Search));if(!string.IsNullOrWhiteSpace(p.Status))q=q.Where(x=>x.Status==p.Status);return Ok(await q.OrderBy(x=>x.Name).Select(x=>new RoleDto(x.Id,x.Name,x.Description,x.Permissions,x.Users.Count,x.Status)).ToPagedResultAsync(p.Page,p.PageSize,ct));}
 [HttpPost] public async Task<ActionResult<RoleDto>> Create(RoleUpsertDto d,CancellationToken ct){var x=new Role{Name=d.Name.Trim(),Description=d.Description,Permissions=d.Permissions,Status=d.Status};db.Roles.Add(x);await db.SaveChangesAsync(ct);return Created("/api/roles/"+x.Id,new RoleDto(x.Id,x.Name,x.Description,x.Permissions,0,x.Status));}
 [HttpPut("{id:guid}")] public async Task<ActionResult> Update(Guid id,RoleUpsertDto d,CancellationToken ct){var x=await db.Roles.FindAsync([id],ct);if(x is null)return NotFound();x.Name=d.Name.Trim();x.Description=d.Description;x.Permissions=d.Permissions;x.Status=d.Status;await db.SaveChangesAsync(ct);return NoContent();}
 [HttpDelete("{id:guid}")] public async Task<ActionResult> Delete(Guid id,CancellationToken ct){var x=await db.Roles.Include(x=>x.Users).FirstOrDefaultAsync(x=>x.Id==id,ct);if(x is null)return NotFound();if(x.Users.Count>0)return Conflict(new ApiMessage("Role cannot be deleted while assigned to users."));db.Roles.Remove(x);await db.SaveChangesAsync(ct);return NoContent();}
}
