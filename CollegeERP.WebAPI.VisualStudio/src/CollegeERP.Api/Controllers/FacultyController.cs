using CollegeERP.Api.Data; using CollegeERP.Api.DTOs; using CollegeERP.Api.Extensions; using CollegeERP.Api.Models; using CollegeERP.Api.Services;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CollegeERP.Api.Controllers;
[ApiController,Route("api/faculty"),Authorize(Roles="Admin,Principal,HOD")]
public sealed class FacultyController(AppDbContext db,CurrentUserService user):ControllerBase
{
    private IQueryable<Faculty> Accessible()
    {
        if (user.Role == "HOD")
        {
            return db.Faculty.Where(
                faculty => faculty.Department.Code == user.Department
            );
        }

        if (user.Role == "Faculty" && user.FacultyId.HasValue)
        {
            return db.Faculty.Where(
                faculty => faculty.Id == user.FacultyId.Value
            );
        }

        return db.Faculty;
    }
    [HttpGet] public async Task<ActionResult<PagedResult<FacultyDto>>> Get([FromQuery] EntityQueryParameters p,CancellationToken ct){var q=Accessible().AsNoTracking();if(!string.IsNullOrWhiteSpace(p.Search))q=q.Where(x=>x.Name.Contains(p.Search)||x.Email.Contains(p.Search)||x.Designation.Contains(p.Search));if(!string.IsNullOrWhiteSpace(p.Department))q=q.Where(x=>x.Department.Code==p.Department);if(!string.IsNullOrWhiteSpace(p.Status))q=q.Where(x=>x.Status==p.Status);return Ok(await q.OrderBy(x=>x.Name).Select(x=>new FacultyDto(x.Id,x.Name,x.Email,x.Department.Code,x.Designation,x.Qualification,x.Phone,x.JoinDate,x.Status)).ToPagedResultAsync(p.Page,p.PageSize,ct));}
 [HttpGet("{id:guid}")] public async Task<ActionResult<FacultyDto>> GetById(Guid id,CancellationToken ct){var x=await Accessible().AsNoTracking().Where(x=>x.Id==id).Select(x=>new FacultyDto(x.Id,x.Name,x.Email,x.Department.Code,x.Designation,x.Qualification,x.Phone,x.JoinDate,x.Status)).FirstOrDefaultAsync(ct);return x is null?NotFound():Ok(x);}
 [HttpPost,Authorize(Roles="Admin,Principal")] public async Task<ActionResult<FacultyDto>> Create(FacultyUpsertDto d,CancellationToken ct){var code=d.Department.Trim().ToUpperInvariant();var dep=await db.Departments.FirstOrDefaultAsync(x=>x.Code==code,ct);if(dep is null)return BadRequest(new ApiMessage("Department code is invalid."));var x=new Faculty{Name=d.Name.Trim(),Email=d.Email.Trim().ToLowerInvariant(),Department=dep,Designation=d.Designation,Qualification=d.Qualification,Phone=d.Phone,JoinDate=d.JoinDate,Status=d.Status};db.Faculty.Add(x);await db.SaveChangesAsync(ct);return CreatedAtAction(nameof(GetById),new{id=x.Id},new FacultyDto(x.Id,x.Name,x.Email,dep.Code,x.Designation,x.Qualification,x.Phone,x.JoinDate,x.Status));}
 [HttpPut("{id:guid}"),Authorize(Roles="Admin,Principal")] public async Task<ActionResult> Update(Guid id,FacultyUpsertDto d,CancellationToken ct){var x=await db.Faculty.FindAsync([id],ct);if(x is null)return NotFound();var code=d.Department.Trim().ToUpperInvariant();var dep=await db.Departments.FirstOrDefaultAsync(a=>a.Code==code,ct);if(dep is null)return BadRequest(new ApiMessage("Department code is invalid."));x.Name=d.Name.Trim();x.Email=d.Email.Trim().ToLowerInvariant();x.Department=dep;x.Designation=d.Designation;x.Qualification=d.Qualification;x.Phone=d.Phone;x.JoinDate=d.JoinDate;x.Status=d.Status;await db.SaveChangesAsync(ct);return NoContent();}
 [HttpDelete("{id:guid}"),Authorize(Roles="Admin,Principal")] public async Task<ActionResult> Delete(Guid id,CancellationToken ct){var x=await db.Faculty.FindAsync([id],ct);if(x is null)return NotFound();db.Faculty.Remove(x);await db.SaveChangesAsync(ct);return NoContent();}
}
