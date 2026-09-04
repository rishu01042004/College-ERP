namespace CollegeERP.Api.Models;

public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Department : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string HodName { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string Description { get; set; } = string.Empty;
    public ICollection<Course> Courses { get; set; } = [];
    public ICollection<Subject> Subjects { get; set; } = [];
    public ICollection<Student> Students { get; set; } = [];
    public ICollection<Faculty> FacultyMembers { get; set; } = [];
}

public sealed class Course : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public string Duration { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public string Status { get; set; } = "Active";
}

public sealed class Subject : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public int SemesterNumber { get; set; }
    public int Credits { get; set; }
    public string Type { get; set; } = "Core";
    public string Status { get; set; } = "Active";
}

public sealed class Semester : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
    public ICollection<Exam> Exams { get; set; } = [];
}

public sealed class Exam : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid? SemesterId { get; set; }
    public Semester? Semester { get; set; }
    public string SemesterName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "Upcoming";
}

public sealed class Student : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public int SemesterNumber { get; set; }
    public string RollNo { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string FeeStatus { get; set; } = "Pending";
}

public sealed class Faculty : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateOnly JoinDate { get; set; }
    public string Status { get; set; } = "Active";
}

public sealed class Role : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public ICollection<UserAccount> Users { get; set; } = [];
}

public sealed class UserAccount : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }
    public Guid? FacultyId { get; set; }
    public Faculty? Faculty { get; set; }
    public string Avatar { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public DateTime? LastLoginUtc { get; set; }
}

public sealed class AttendanceRecord : EntityBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public DateOnly Date { get; set; }
    public string Status { get; set; } = "Present";
    public Guid? MarkedByFacultyId { get; set; }
    public Faculty? MarkedByFaculty { get; set; }
    public string MarkedByName { get; set; } = string.Empty;
}

public sealed class MarkRecord : EntityBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public Guid SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public string ExamName { get; set; } = string.Empty;
    public decimal InternalMarks { get; set; }
    public decimal ExternalMarks { get; set; }
    public decimal MaxMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
}

public sealed class ResultRecord : EntityBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public string SemesterName { get; set; } = string.Empty;
    public decimal Sgpa { get; set; }
    public int TotalCredits { get; set; }
    public string Status { get; set; } = "Passed";
    public string Year { get; set; } = string.Empty;
}

public sealed class FeeRecord : EntityBase
{
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
    public string SemesterName { get; set; } = string.Empty;
    public decimal TotalFee { get; set; }
    public decimal Paid { get; set; }
    public string Status { get; set; } = "Pending";
    public DateOnly? PaidDate { get; set; }
    public string Mode { get; set; } = string.Empty;
}

public sealed class BookCategory : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public ICollection<Book> Books { get; set; } = [];
}

public sealed class Book : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public BookCategory Category { get; set; } = null!;
    public int Copies { get; set; }
    public int Available { get; set; }
    public string Status { get; set; } = "Available";
}

public sealed class Announcement : EntityBase
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Priority { get; set; } = "Medium";
    public string Target { get; set; } = "All";
    public Guid? PostedByUserId { get; set; }
    public UserAccount? PostedByUser { get; set; }
    public string PostedByName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Status { get; set; } = "Active";
}

public sealed class Notification : EntityBase
{
    public Guid? UserId { get; set; }
    public UserAccount? User { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime DateUtc { get; set; }
}

public sealed class LoginHistory : EntityBase
{
    public Guid? UserId { get; set; }
    public UserAccount? User { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
    public DateTime TimeUtc { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class HodAuthorization : EntityBase
{
    public string Type { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    public string Details { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateOnly RequestDate { get; set; }
    public Guid? DecidedByUserId { get; set; }
    public UserAccount? DecidedByUser { get; set; }
    public DateTime? DecidedAtUtc { get; set; }
}

public sealed class SystemSetting : EntityBase
{
    public string CollegeName { get; set; } = "National Institute of Technology";
    public string AcademicYear { get; set; } = "2024-25";
    public string PrincipalName { get; set; } = "Dr. Anand Sharma";
    public string DefaultSemester { get; set; } = "Semester 2";
    public int AttendanceThreshold { get; set; } = 75;
    public int FeeReminderDays { get; set; } = 15;
}
