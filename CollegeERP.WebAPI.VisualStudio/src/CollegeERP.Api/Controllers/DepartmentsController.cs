using CollegeERP.Api.Data;
using CollegeERP.Api.DTOs;
using CollegeERP.Api.Extensions;
using CollegeERP.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Controllers;

[ApiController, Route("api/departments"), Authorize(Roles = "Admin,Principal")]
public sealed class DepartmentsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<DepartmentDto>>> Get([FromQuery] EntityQueryParameters p, CancellationToken ct)
    {
        var q = db.Departments.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(p.Search)) q = q.Where(x => x.Name.Contains(p.Search) || x.Code.Contains(p.Search) || x.HodName.Contains(p.Search));
        if (!string.IsNullOrWhiteSpace(p.Status)) q = q.Where(x => x.Status == p.Status);
        return Ok(await q.OrderBy(x => x.Name).Select(x => new DepartmentDto(x.Id, x.Name, x.Code, x.HodName, x.FacultyMembers.Count, x.Students.Count, x.Status, x.Description)).ToPagedResultAsync(p.Page, p.PageSize, ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DepartmentDto>> GetById(Guid id, CancellationToken ct)
    {
        var item = await db.Departments.AsNoTracking().Where(x => x.Id == id).Select(x => new DepartmentDto(x.Id, x.Name, x.Code, x.HodName, x.FacultyMembers.Count, x.Students.Count, x.Status, x.Description)).FirstOrDefaultAsync(ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(
      DepartmentUpsertDto dto,
      CancellationToken ct)
    {
        var name = dto.Name?.Trim() ?? "";
        var code = dto.Code?.Trim().ToUpperInvariant() ?? "";

        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new
            {
                message = "Department name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new
            {
                message = "Department code is required."
            });
        }

        var codeExists = await db.Departments
            .AnyAsync(
                department => department.Code == code,
                ct
            );

        if (codeExists)
        {
            return Conflict(new
            {
                message = $"Department code '{code}' already exists."
            });
        }

        var item = new Department
        {
            Name = name,
            Code = code,
            HodName = dto.Hod?.Trim() ?? "",
            Status = string.IsNullOrWhiteSpace(dto.Status)
                ? "Active"
                : dto.Status.Trim(),
            Description = dto.Description?.Trim() ?? ""
        };

        db.Departments.Add(item);
        await db.SaveChangesAsync(ct);

        var response = new DepartmentDto(
            item.Id,
            item.Name,
            item.Code,
            item.HodName,
            0,
            0,
            item.Status,
            item.Description
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = item.Id },
            response
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(
        Guid id,
        DepartmentUpsertDto dto,
        CancellationToken ct)
    {
        var item = await db.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                ct
            );

        if (item is null)
        {
            return NotFound(new
            {
                message = "Department not found."
            });
        }

        var name = dto.Name?.Trim() ?? "";
        var code = dto.Code?.Trim().ToUpperInvariant() ?? "";

        var duplicateCode = await db.Departments
            .AnyAsync(
                department =>
                    department.Code == code &&
                    department.Id != id,
                ct
            );

        if (duplicateCode)
        {
            return Conflict(new
            {
                message = $"Department code '{code}' already exists."
            });
        }

        item.Name = name;
        item.Code = code;
        item.HodName = dto.Hod?.Trim() ?? "";
        item.Status = dto.Status?.Trim() ?? "Active";
        item.Description = dto.Description?.Trim() ?? "";

        await db.SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var item = await db.Departments.Include(x => x.Courses).Include(x => x.Subjects).Include(x => x.Students).Include(x => x.FacultyMembers).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item is null) return NotFound();
        if (item.Courses.Count + item.Subjects.Count + item.Students.Count + item.FacultyMembers.Count > 0) return Conflict(new ApiMessage("Department cannot be deleted while related records exist."));
        db.Departments.Remove(item); await db.SaveChangesAsync(ct); return NoContent();
    }
}
