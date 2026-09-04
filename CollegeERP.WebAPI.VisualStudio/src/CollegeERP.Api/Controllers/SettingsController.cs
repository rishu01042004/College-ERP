using CollegeERP.Api.Data; using CollegeERP.Api.DTOs;
using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CollegeERP.Api.Controllers;
[ApiController,Route("api/settings"),Authorize(Roles="Admin")]
public sealed class SettingsController(AppDbContext db):ControllerBase
{
 [HttpGet] public async Task<ActionResult<SystemSettingDto>> Get(CancellationToken ct){var x=await db.SystemSettings.AsNoTracking().FirstAsync(ct);return Ok(new SystemSettingDto(x.Id,x.CollegeName,x.AcademicYear,x.PrincipalName,x.DefaultSemester,x.AttendanceThreshold,x.FeeReminderDays));}
 [HttpPut] public async Task<ActionResult> Update(SystemSettingUpdateDto d,CancellationToken ct){var x=await db.SystemSettings.FirstAsync(ct);x.CollegeName=d.CollegeName;x.AcademicYear=d.AcademicYear;x.PrincipalName=d.PrincipalName;x.DefaultSemester=d.DefaultSemester;x.AttendanceThreshold=d.AttendanceThreshold;x.FeeReminderDays=d.FeeReminderDays;await db.SaveChangesAsync(ct);return NoContent();}
}
