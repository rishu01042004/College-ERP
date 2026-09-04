using CollegeERP.Api.Data; using CollegeERP.Api.DTOs; using CollegeERP.Api.Extensions; using CollegeERP.Api.Models;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CollegeERP.Api.Controllers;
[ApiController,Route("api/categories"),Authorize(Roles="Admin,Principal")]
public sealed class CategoriesController(AppDbContext db):ControllerBase
{
 [HttpGet] public async Task<ActionResult<PagedResult<BookCategoryDto>>> Get([FromQuery] EntityQueryParameters p,CancellationToken ct){var q=db.BookCategories.AsNoTracking().AsQueryable();if(!string.IsNullOrWhiteSpace(p.Search))q=q.Where(x=>x.Name.Contains(p.Search)||x.Code.Contains(p.Search));if(!string.IsNullOrWhiteSpace(p.Status))q=q.Where(x=>x.Status==p.Status);return Ok(await q.OrderBy(x=>x.Name).Select(x=>new BookCategoryDto(x.Id,x.Name,x.Code,x.Books.Count,x.Status)).ToPagedResultAsync(p.Page,p.PageSize,ct));}
 [HttpGet("{id:guid}")] public async Task<ActionResult<BookCategoryDto>> GetById(Guid id,CancellationToken ct){var x=await db.BookCategories.AsNoTracking().Where(x=>x.Id==id).Select(x=>new BookCategoryDto(x.Id,x.Name,x.Code,x.Books.Count,x.Status)).FirstOrDefaultAsync(ct);return x is null?NotFound():Ok(x);}
 [HttpPost] public async Task<ActionResult<BookCategoryDto>> Create(BookCategoryUpsertDto d,CancellationToken ct){var x=new BookCategory{Name=d.Name.Trim(),Code=d.Code.Trim().ToUpperInvariant(),Status=d.Status};db.BookCategories.Add(x);await db.SaveChangesAsync(ct);return CreatedAtAction(nameof(GetById),new{id=x.Id},new BookCategoryDto(x.Id,x.Name,x.Code,0,x.Status));}
 [HttpPut("{id:guid}")] public async Task<ActionResult> Update(Guid id,BookCategoryUpsertDto d,CancellationToken ct){var x=await db.BookCategories.FindAsync([id],ct);if(x is null)return NotFound();x.Name=d.Name.Trim();x.Code=d.Code.Trim().ToUpperInvariant();x.Status=d.Status;await db.SaveChangesAsync(ct);return NoContent();}
 [HttpDelete("{id:guid}")] public async Task<ActionResult> Delete(Guid id,CancellationToken ct){var x=await db.BookCategories.Include(x=>x.Books).FirstOrDefaultAsync(x=>x.Id==id,ct);if(x is null)return NotFound();if(x.Books.Count>0)return Conflict(new ApiMessage("Category cannot be deleted while books exist."));db.BookCategories.Remove(x);await db.SaveChangesAsync(ct);return NoContent();}
}
