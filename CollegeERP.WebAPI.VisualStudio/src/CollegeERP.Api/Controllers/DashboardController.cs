using CollegeERP.Api.Data;
using CollegeERP.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController(AppDbContext db, CurrentUserService user) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken ct)
    {
        var students = db.Students.AsNoTracking();
        var faculty = db.Faculty.AsNoTracking();
        var attendance = db.Attendance
            .AsNoTracking()
            .Include(x => x.Student).ThenInclude(x => x.Department)
            .AsQueryable();

        var results = db.Results
            .AsNoTracking()
            .Include(x => x.Student).ThenInclude(x => x.Department)
            .AsQueryable();

        if (user.Role == "HOD")
        {
            students = students.Where(x => x.Department.Code == user.Department);
            faculty = faculty.Where(x => x.Department.Code == user.Department);
            attendance = attendance.Where(x => x.Student.Department.Code == user.Department);
            results = results.Where(x => x.Student.Department.Code == user.Department);
        }
        else if (user.Role == "Faculty")
        {
            students = students.Where(x => x.Department.Code == user.Department);
            faculty = faculty.Where(x => x.Department.Code == user.Department);
            attendance = attendance.Where(x => x.Student.Department.Code == user.Department || x.MarkedByFacultyId == user.FacultyId);
            results = results.Where(x => x.Student.Department.Code == user.Department);
        }
        else if (user.Role == "Student" && user.StudentId is Guid studentId)
        {
            students = students.Where(x => x.Id == studentId);
            faculty = faculty.Where(x => false);
            attendance = attendance.Where(x => x.StudentId == studentId);
            results = results.Where(x => x.StudentId == studentId);
        }

        var totalStudents = await students.CountAsync(ct);
        var totalFaculty = await faculty.CountAsync(ct);
        var totalDepartments = user.IsInRole("Admin", "Principal") ? await db.Departments.CountAsync(ct) : (string.IsNullOrEmpty(user.Department) ? 0 : 1);
        var attendanceRows = await attendance.ToListAsync(ct);
        var attendancePercentage = attendanceRows.Count == 0 ? 0 : Math.Round(attendanceRows.Count(x => x.Status == "Present") * 100d / attendanceRows.Count, 1);
        var averageSgpa = await results.AnyAsync(ct) ? Math.Round(await results.AverageAsync(x => (double)x.Sgpa, ct), 2) : 0;

        var announcementsQuery = db.Announcements.AsNoTracking().Where(x => x.Status == "Active");
        if (user.Role == "Student") announcementsQuery = announcementsQuery.Where(x => x.Target == "All" || x.Target == "Students");
        if (user.Role == "Faculty") announcementsQuery = announcementsQuery.Where(x => x.Target == "All" || x.Target == "Faculty");
        if (user.Role == "HOD") announcementsQuery = announcementsQuery.Where(x => x.Target == "All" || x.Target == "HOD" || x.Target == "Faculty");

        var recentAnnouncements = await announcementsQuery.OrderByDescending(x => x.Date).Take(5)
            .Select(x => new { x.Id, x.Title, x.Priority, x.Target, x.Date, postedBy = x.PostedByName }).ToListAsync(ct);

        var notifications = db.Notifications.AsNoTracking();
        if (!user.IsInRole("Admin", "Principal"))
            notifications = notifications.Where(x => x.UserId == user.UserId || x.RecipientName == user.Name);
        var unreadNotifications = await notifications.CountAsync(x => !x.IsRead, ct);

        return Ok(new
        {
            role = user.Role,
            department = user.Department,
            stats = new { totalStudents, totalFaculty, totalDepartments, attendancePercentage, averageSgpa, unreadNotifications },
            recentAnnouncements
        });
    }
}
