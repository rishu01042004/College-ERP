using System.ComponentModel.DataAnnotations;

namespace CollegeERP.Api.DTOs;

public sealed record DepartmentDto(Guid Id, string Name, string Code, string Hod, int FacultyCount, int StudentCount, string Status, string Description);
public sealed class DepartmentUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; public string Hod { get; set; } = ""; public string Status { get; set; } = "Active"; public string Description { get; set; } = ""; }

public sealed record CourseDto(Guid Id, string Name, string Code, string Department, string Duration, int TotalSeats, string Status);
public sealed class CourseUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; [Required] public string Department { get; set; } = ""; public string Duration { get; set; } = ""; [Range(0, 10000)] public int TotalSeats { get; set; } = 60; public string Status { get; set; } = "Active"; }

public sealed record SubjectDto(Guid Id, string Name, string Code, string Department, string Semester, int Credits, string Type, string Status);
public sealed class SubjectUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; [Required] public string Department { get; set; } = ""; [Range(1, 20)] public int Semester { get; set; } [Range(0, 30)] public int Credits { get; set; } public string Type { get; set; } = "Core"; public string Status { get; set; } = "Active"; }

public sealed record SemesterDto(Guid Id, string Name, string Code, string AcademicYear, DateOnly StartDate, DateOnly EndDate, string Status);
public sealed class SemesterUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; [Required] public string AcademicYear { get; set; } = ""; public DateOnly StartDate { get; set; } public DateOnly EndDate { get; set; } public string Status { get; set; } = "Upcoming"; }

public sealed record ExamDto(Guid Id, string Name, string Code, string Semester, DateOnly StartDate, DateOnly EndDate, string Type, string Status);
public sealed class ExamUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; [Required] public string Semester { get; set; } = ""; public DateOnly StartDate { get; set; } public DateOnly EndDate { get; set; } public string Type { get; set; } = ""; public string Status { get; set; } = "Upcoming"; }

public sealed record StudentDto(Guid Id, string Name, string Email, string Department, string Semester, string RollNo, string Phone, DateOnly Dob, string Gender, string Status, string FeeStatus);
public sealed class StudentUpsertDto { [Required] public string Name { get; set; } = ""; [Required, EmailAddress] public string Email { get; set; } = ""; [Required] public string Department { get; set; } = ""; [Range(1,20)] public int Semester { get; set; } [Required] public string RollNo { get; set; } = ""; public string Phone { get; set; } = ""; public DateOnly Dob { get; set; } public string Gender { get; set; } = ""; public string Status { get; set; } = "Active"; public string FeeStatus { get; set; } = "Pending"; }

public sealed record FacultyDto(Guid Id, string Name, string Email, string Department, string Designation, string Qualification, string Phone, DateOnly JoinDate, string Status);
public sealed class FacultyUpsertDto { [Required] public string Name { get; set; } = ""; [Required, EmailAddress] public string Email { get; set; } = ""; [Required] public string Department { get; set; } = ""; public string Designation { get; set; } = ""; public string Qualification { get; set; } = ""; public string Phone { get; set; } = ""; public DateOnly JoinDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); public string Status { get; set; } = "Active"; }

public sealed record RoleDto(Guid Id, string Name, string Description, string Permissions, int UserCount, string Status);
public sealed class RoleUpsertDto { [Required] public string Name { get; set; } = ""; public string Description { get; set; } = ""; public string Permissions { get; set; } = ""; public string Status { get; set; } = "Active"; }

public sealed record UserDto(Guid Id, string Name, string Email, string Role, string Department, string Status, DateTime? LastLogin, string Avatar, string Color);
public sealed class UserCreateDto { [Required] public string Name { get; set; } = ""; [Required, EmailAddress] public string Email { get; set; } = ""; [Required, MinLength(8)] public string Password { get; set; } = ""; [Required] public string Role { get; set; } = ""; public string? Department { get; set; } public string Status { get; set; } = "Active"; public string Avatar { get; set; } = ""; public string Color { get; set; } = ""; }
public sealed class UserUpdateDto { [Required] public string Name { get; set; } = ""; [Required] public string Role { get; set; } = ""; public string? Department { get; set; } public string Status { get; set; } = "Active"; public string Avatar { get; set; } = ""; public string Color { get; set; } = ""; }

public sealed record AttendanceDto(Guid Id, string Student, string RollNo, string Subject, DateOnly Date, string Status, string MarkedBy);
public sealed class AttendanceUpsertDto { [Required] public Guid StudentId { get; set; } [Required] public Guid SubjectId { get; set; } public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow); public string Status { get; set; } = "Present"; public Guid? MarkedByFacultyId { get; set; } public string MarkedBy { get; set; } = ""; }

public sealed record MarkDto(Guid Id, string Student, string RollNo, string Subject, string Exam, decimal Internal, decimal External, decimal Total, decimal MaxMarks, string Grade);
public sealed class MarkUpsertDto { [Required] public Guid StudentId { get; set; } [Required] public Guid SubjectId { get; set; } [Required] public string Exam { get; set; } = ""; [Range(0,1000)] public decimal Internal { get; set; } [Range(0,1000)] public decimal External { get; set; } [Range(1,1000)] public decimal MaxMarks { get; set; } = 100; public string Grade { get; set; } = ""; }

public sealed record ResultDto(Guid Id, string Student, string RollNo, string Semester, decimal Sgpa, int TotalCredits, string Status, string Year);
public sealed class ResultUpsertDto { [Required] public Guid StudentId { get; set; } [Required] public string Semester { get; set; } = ""; [Range(0,10)] public decimal Sgpa { get; set; } [Range(0,100)] public int TotalCredits { get; set; } public string Status { get; set; } = "Passed"; public string Year { get; set; } = ""; }

public sealed record FeeDto(Guid Id, string Student, string RollNo, string Semester, decimal TotalFee, decimal Paid, decimal Due, string Status, DateOnly? PaidDate, string Mode);
public sealed class FeeUpsertDto { [Required] public Guid StudentId { get; set; } [Required] public string Semester { get; set; } = ""; [Range(0,double.MaxValue)] public decimal TotalFee { get; set; } [Range(0,double.MaxValue)] public decimal Paid { get; set; } public string Status { get; set; } = "Pending"; public DateOnly? PaidDate { get; set; } public string Mode { get; set; } = ""; }

public sealed record BookCategoryDto(Guid Id, string Name, string Code, int BookCount, string Status);
public sealed class BookCategoryUpsertDto { [Required] public string Name { get; set; } = ""; [Required] public string Code { get; set; } = ""; public string Status { get; set; } = "Active"; }

public sealed record BookDto(Guid Id, string Title, string Isbn, string Author, string Category, int Copies, int Available, string Status);
public sealed class BookUpsertDto { [Required] public string Title { get; set; } = ""; [Required] public string Isbn { get; set; } = ""; [Required] public string Author { get; set; } = ""; [Required] public string Category { get; set; } = ""; [Range(0,100000)] public int Copies { get; set; } [Range(0,100000)] public int Available { get; set; } public string Status { get; set; } = "Available"; }

public sealed record AnnouncementDto(Guid Id, string Title, string Content, string Priority, string Target, string PostedBy, DateOnly Date, string Status);
public sealed class AnnouncementUpsertDto { [Required] public string Title { get; set; } = ""; public string Content { get; set; } = ""; public string Priority { get; set; } = "Medium"; public string Target { get; set; } = "All"; public DateOnly? Date { get; set; } public string Status { get; set; } = "Active"; }

public sealed record NotificationDto(Guid Id, string Title, string Message, string User, string Type, bool Read, DateTime Date);
public sealed class NotificationCreateDto { public Guid? UserId { get; set; } [Required] public string User { get; set; } = ""; [Required] public string Title { get; set; } = ""; [Required] public string Message { get; set; } = ""; public string Type { get; set; } = "General"; }

public sealed record LoginHistoryDto(Guid Id, string User, string Email, string Ip, string Device, DateTime Time, string Status);

public sealed record HodAuthorizationDto(Guid Id, string Type, string RequestedBy, string Department, string Details, string Status, DateOnly RequestDate);
public sealed class HodAuthorizationCreateDto { [Required] public string Type { get; set; } = ""; [Required] public string RequestedBy { get; set; } = ""; [Required] public string Department { get; set; } = ""; public string Details { get; set; } = ""; public DateOnly? RequestDate { get; set; } }
public sealed record AuthorizationDecisionDto([property: Required] string Decision);

public sealed record SystemSettingDto(Guid Id, string CollegeName, string AcademicYear, string PrincipalName, string DefaultSemester, int AttendanceThreshold, int FeeReminderDays);
public sealed class SystemSettingUpdateDto { [Required] public string CollegeName { get; set; } = ""; [Required] public string AcademicYear { get; set; } = ""; [Required] public string PrincipalName { get; set; } = ""; [Required] public string DefaultSemester { get; set; } = ""; [Range(0,100)] public int AttendanceThreshold { get; set; } = 75; [Range(0,365)] public int FeeReminderDays { get; set; } = 15; }
