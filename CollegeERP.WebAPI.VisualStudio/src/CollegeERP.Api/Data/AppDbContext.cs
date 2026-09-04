using CollegeERP.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Faculty> Faculty => Set<Faculty>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<AttendanceRecord> Attendance => Set<AttendanceRecord>();
    public DbSet<MarkRecord> Marks => Set<MarkRecord>();
    public DbSet<ResultRecord> Results => Set<ResultRecord>();
    public DbSet<FeeRecord> Fees => Set<FeeRecord>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<LoginHistory> LoginHistory => Set<LoginHistory>();
    public DbSet<HodAuthorization> HodAuthorizations => Set<HodAuthorization>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
                modelBuilder.Entity(entityType.ClrType).HasKey(nameof(EntityBase.Id));
        }

        modelBuilder.Entity<Department>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Course>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Subject>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Semester>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Exam>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Student>().HasIndex(x => x.RollNo).IsUnique();
        modelBuilder.Entity<Faculty>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<UserAccount>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<BookCategory>().HasIndex(x => x.Code).IsUnique();
        modelBuilder.Entity<Book>().HasIndex(x => x.Isbn).IsUnique();

        modelBuilder.Entity<Course>().HasOne(x => x.Department).WithMany(x => x.Courses).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Subject>().HasOne(x => x.Department).WithMany(x => x.Subjects).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Student>().HasOne(x => x.Department).WithMany(x => x.Students).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Faculty>().HasOne(x => x.Department).WithMany(x => x.FacultyMembers).HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserAccount>().HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<UserAccount>().HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<UserAccount>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<UserAccount>().HasOne(x => x.Faculty).WithMany().HasForeignKey(x => x.FacultyId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<AttendanceRecord>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<AttendanceRecord>().HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<AttendanceRecord>().HasOne(x => x.MarkedByFaculty).WithMany().HasForeignKey(x => x.MarkedByFacultyId).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<MarkRecord>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<MarkRecord>().HasOne(x => x.Subject).WithMany().HasForeignKey(x => x.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ResultRecord>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FeeRecord>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Book>().HasOne(x => x.Category).WithMany(x => x.Books).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HodAuthorization>().HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MarkRecord>().Property(x => x.InternalMarks).HasPrecision(10, 2);
        modelBuilder.Entity<MarkRecord>().Property(x => x.ExternalMarks).HasPrecision(10, 2);
        modelBuilder.Entity<MarkRecord>().Property(x => x.MaxMarks).HasPrecision(10, 2);
        modelBuilder.Entity<ResultRecord>().Property(x => x.Sgpa).HasPrecision(4, 2);
        modelBuilder.Entity<FeeRecord>().Property(x => x.TotalFee).HasPrecision(18, 2);
        modelBuilder.Entity<FeeRecord>().Property(x => x.Paid).HasPrecision(18, 2);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.UpdatedAtUtc = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
