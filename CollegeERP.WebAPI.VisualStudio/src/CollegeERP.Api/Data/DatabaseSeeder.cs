using System.Globalization;
using System.Text.Json;
using CollegeERP.Api.Models;
using CollegeERP.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Data;

public sealed class DatabaseSeeder(AppDbContext db, PasswordService passwords, IWebHostEnvironment environment)
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if (await db.Departments.AnyAsync(ct)) return;

        var seedPath = Path.Combine(environment.ContentRootPath, "Data", "Seed", "seed-data.json");
        await using var stream = File.OpenRead(seedPath);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var root = json.RootElement;

        var departments = root.GetProperty("departments").EnumerateArray().Select(x => new Department
        {
            Name = S(x, "name"), Code = S(x, "code"), HodName = S(x, "hod"), Status = S(x, "status"), Description = S(x, "description")
        }).ToList();
        db.Departments.AddRange(departments);
        var deptByCode = departments.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

        var semesters = root.GetProperty("semesters").EnumerateArray().Select(x => new Semester
        {
            Name = S(x, "name"), Code = S(x, "code"), AcademicYear = S(x, "academicYear"),
            StartDate = D(x, "startDate"), EndDate = D(x, "endDate"), Status = S(x, "status")
        }).ToList();
        db.Semesters.AddRange(semesters);
        var semesterByName = semesters.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        var students = root.GetProperty("students").EnumerateArray().Select(x => new Student
        {
            Name = S(x, "name"), Email = S(x, "email"), Department = deptByCode[S(x, "department")],
            SemesterNumber = I(x, "semester"), RollNo = S(x, "rollNo"), Phone = S(x, "phone"),
            DateOfBirth = D(x, "dob"), Gender = S(x, "gender"), Status = S(x, "status"), FeeStatus = S(x, "feeStatus")
        }).ToList();
        db.Students.AddRange(students);
        var studentByRoll = students.ToDictionary(x => x.RollNo, StringComparer.OrdinalIgnoreCase);
        var studentByEmail = students.ToDictionary(x => x.Email, StringComparer.OrdinalIgnoreCase);

        var faculty = root.GetProperty("faculty").EnumerateArray().Select(x => new Faculty
        {
            Name = S(x, "name"), Email = S(x, "email"), Department = deptByCode[S(x, "department")],
            Designation = S(x, "designation"), Qualification = S(x, "qualification"), Phone = S(x, "phone"),
            JoinDate = D(x, "joinDate"), Status = S(x, "status")
        }).ToList();
        db.Faculty.AddRange(faculty);
        var facultyByName = faculty.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        var facultyByEmail = faculty.ToDictionary(x => x.Email, StringComparer.OrdinalIgnoreCase);

        var roles = root.GetProperty("roles").EnumerateArray().Select(x => new Role
        {
            Name = S(x, "name"), Description = S(x, "description"), Permissions = S(x, "permissions"), Status = S(x, "status")
        }).ToList();
        db.Roles.AddRange(roles);
        var roleByName = roles.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        var courses = root.GetProperty("courses").EnumerateArray().Select(x => new Course
        {
            Name = S(x, "name"), Code = S(x, "code"), Department = deptByCode[S(x, "department")],
            Duration = S(x, "duration"), TotalSeats = I(x, "totalSeats"), Status = S(x, "status")
        }).ToList();
        db.Courses.AddRange(courses);

        var subjects = root.GetProperty("subjects").EnumerateArray().Select(x => new Subject
        {
            Name = S(x, "name"), Code = S(x, "code"), Department = deptByCode[S(x, "department")],
            SemesterNumber = I(x, "semester"), Credits = I(x, "credits"), Type = S(x, "type"), Status = S(x, "status")
        }).ToList();
        db.Subjects.AddRange(subjects);
        var subjectByName = subjects.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        db.Exams.AddRange(root.GetProperty("exams").EnumerateArray().Select(x =>
        {
            var semesterName = S(x, "semester");
            return new Exam
            {
                Name = S(x, "name"), Code = S(x, "code"), SemesterName = semesterName,
                Semester = semesterByName.GetValueOrDefault(semesterName), StartDate = D(x, "startDate"),
                EndDate = D(x, "endDate"), Type = S(x, "type"), Status = S(x, "status")
            };
        }));

        var categories = root.GetProperty("categories").EnumerateArray().Select(x => new BookCategory
        {
            Name = S(x, "name"), Code = S(x, "code"), Status = S(x, "status")
        }).ToList();
        db.BookCategories.AddRange(categories);
        var categoryByName = categories.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        db.Books.AddRange(root.GetProperty("books").EnumerateArray().Select(x => new Book
        {
            Title = S(x, "title"), Isbn = S(x, "isbn"), Author = S(x, "author"),
            Category = categoryByName[S(x, "category")], Copies = I(x, "copies"), Available = I(x, "available"), Status = S(x, "status")
        }));

        var lastLoginByEmail = root.GetProperty("users").EnumerateArray()
            .ToDictionary(x => S(x, "email"), x => ParseDateTime(S(x, "lastLogin")), StringComparer.OrdinalIgnoreCase);

        var demoAccounts = new[]
        {
            new DemoAccount("admin@collegeerp.com", "admin123", "System Admin", "Admin", "", "SA", "#10b981"),
            new DemoAccount("principal@college.edu", "principal123", "Dr. Anand Sharma", "Principal", "", "AS", "#8b5cf6"),
            new DemoAccount("ramesh.k@college.edu", "hod123", "Dr. Ramesh Kumar", "HOD", "CSE", "RK", "#06b6d4"),
            new DemoAccount("priya.s@college.edu", "hod123", "Dr. Priya Sharma", "HOD", "ECE", "PS", "#06b6d4"),
            new DemoAccount("amit.v@college.edu", "faculty123", "Mr. Amit Verma", "Faculty", "CSE", "AV", "#f59e0b"),
            new DemoAccount("neha.a@college.edu", "faculty123", "Ms. Neha Agarwal", "Faculty", "CSE", "NA", "#f59e0b"),
            new DemoAccount("aarav.m@college.edu", "student123", "Aarav Mehta", "Student", "CSE", "AM", "#ec4899"),
            new DemoAccount("diya.s@college.edu", "student123", "Diya Sharma", "Student", "CSE", "DS", "#ec4899"),
            new DemoAccount("rohan.p@college.edu", "student123", "Rohan Patel", "Student", "ECE", "RP", "#ec4899")
        };

        var users = demoAccounts.Select(a => new UserAccount
        {
            Email = a.Email, Name = a.Name, PasswordHash = passwords.Hash(a.Password), Role = roleByName[a.Role],
            Department = string.IsNullOrWhiteSpace(a.Department) ? null : deptByCode[a.Department],
            Student = studentByEmail.GetValueOrDefault(a.Email), Faculty = facultyByEmail.GetValueOrDefault(a.Email),
            Avatar = a.Avatar, Color = a.Color, Status = "Active", LastLoginUtc = lastLoginByEmail.GetValueOrDefault(a.Email)
        }).ToList();
        db.Users.AddRange(users);
        var userByName = users.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

        db.Attendance.AddRange(root.GetProperty("attendance").EnumerateArray().Select(x =>
        {
            var marker = S(x, "markedBy");
            return new AttendanceRecord
            {
                Student = studentByRoll[S(x, "rollNo")], Subject = subjectByName[S(x, "subject")], Date = D(x, "date"),
                Status = S(x, "status"), MarkedByName = marker, MarkedByFaculty = facultyByName.GetValueOrDefault(marker)
            };
        }));

        db.Marks.AddRange(root.GetProperty("marks").EnumerateArray().Select(x => new MarkRecord
        {
            Student = studentByRoll[S(x, "rollNo")], Subject = subjectByName[S(x, "subject")], ExamName = S(x, "exam"),
            InternalMarks = M(x, "internal"), ExternalMarks = M(x, "external"), MaxMarks = M(x, "maxMarks"), Grade = S(x, "grade")
        }));

        db.Results.AddRange(root.GetProperty("results").EnumerateArray().Select(x => new ResultRecord
        {
            Student = studentByRoll[S(x, "rollNo")], SemesterName = S(x, "semester"), Sgpa = M(x, "sgpa"),
            TotalCredits = I(x, "totalCredits"), Status = S(x, "status"), Year = S(x, "year")
        }));

        db.Fees.AddRange(root.GetProperty("fees").EnumerateArray().Select(x => new FeeRecord
        {
            Student = studentByRoll[S(x, "rollNo")], SemesterName = S(x, "semester"), TotalFee = M(x, "totalFee"),
            Paid = M(x, "paid"), Status = S(x, "status"), PaidDate = NullableDate(x, "paidDate"), Mode = S(x, "mode")
        }));

        db.Announcements.AddRange(root.GetProperty("announcements").EnumerateArray().Select(x =>
        {
            var postedBy = S(x, "postedBy");
            return new Announcement
            {
                Title = S(x, "title"), Content = S(x, "content"), Priority = S(x, "priority"), Target = S(x, "target"),
                PostedByName = postedBy, PostedByUser = userByName.GetValueOrDefault(postedBy), Date = D(x, "date"), Status = S(x, "status")
            };
        }));

        db.Notifications.AddRange(root.GetProperty("notifications").EnumerateArray().Select(x =>
        {
            var recipient = S(x, "user");
            return new Notification
            {
                RecipientName = recipient, User = userByName.GetValueOrDefault(recipient), Title = S(x, "title"),
                Message = S(x, "message"), Type = S(x, "type"), IsRead = B(x, "read"), DateUtc = ParseDateTime(S(x, "date")) ?? DateTime.UtcNow
            };
        }));

        db.LoginHistory.AddRange(root.GetProperty("loginHistory").EnumerateArray().Select(x =>
        {
            var email = S(x, "email");
            return new LoginHistory
            {
                User = users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)),
                UserName = S(x, "user"), Email = email, IpAddress = S(x, "ip"), Device = S(x, "device"),
                TimeUtc = ParseDateTime(S(x, "time")) ?? DateTime.UtcNow, Status = S(x, "status")
            };
        }));

        db.HodAuthorizations.AddRange(root.GetProperty("hodAuth").EnumerateArray().Select(x => new HodAuthorization
        {
            Type = S(x, "type"), RequestedBy = S(x, "requestedBy"), Department = deptByCode[S(x, "department")],
            Details = S(x, "details"), Status = S(x, "status"), RequestDate = D(x, "requestDate")
        }));

        db.SystemSettings.Add(new SystemSetting());
        await db.SaveChangesAsync(ct);
    }

    private sealed record DemoAccount(string Email, string Password, string Name, string Role, string Department, string Avatar, string Color);
    private static string S(JsonElement x, string property) => x.GetProperty(property).GetString() ?? string.Empty;
    private static int I(JsonElement x, string property) => x.GetProperty(property).ValueKind == JsonValueKind.String ? int.Parse(S(x, property), CultureInfo.InvariantCulture) : x.GetProperty(property).GetInt32();
    private static decimal M(JsonElement x, string property) => x.GetProperty(property).GetDecimal();
    private static bool B(JsonElement x, string property) => x.GetProperty(property).GetBoolean();
    private static DateOnly D(JsonElement x, string property) => DateOnly.Parse(S(x, property), CultureInfo.InvariantCulture);
    private static DateOnly? NullableDate(JsonElement x, string property) => string.IsNullOrWhiteSpace(S(x, property)) ? null : D(x, property);
    private static DateTime? ParseDateTime(string value) => string.IsNullOrWhiteSpace(value) ? null : DateTime.SpecifyKind(DateTime.Parse(value, CultureInfo.InvariantCulture), DateTimeKind.Utc);
}
