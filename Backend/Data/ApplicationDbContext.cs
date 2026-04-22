// ============================================================
// Data/ApplicationDbContext.cs
//
// This is the EQUIVALENT of your config.php + all SQL queries.
// In PHP you called mysqli_connect() and wrote raw SQL strings.
// Here, EF Core handles the connection AND translates C# LINQ
// expressions into SQL automatically.
//
// Think of DbContext as your "database manager class".
// ============================================================

using Microsoft.EntityFrameworkCore;
using StudentMS.Api.Models;

namespace StudentMS.Api.Data;

public class ApplicationDbContext : DbContext
{
    // Constructor — receives configuration from dependency injection
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Each DbSet = one table in MySQL
    // In PHP these were just implicit from your SELECT * FROM queries
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Section> Sections { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<StudentGrade> StudentGrades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map C# class names → exact MySQL table names from your setup.sql
        modelBuilder.Entity<Student>().ToTable("students");
        modelBuilder.Entity<Course>().ToTable("course");
        modelBuilder.Entity<Section>().ToTable("section");
        modelBuilder.Entity<Gender>().ToTable("gender");
        modelBuilder.Entity<Subject>().ToTable("subject");
        modelBuilder.Entity<StudentGrade>().ToTable("student_grades");

        // Configure ON DELETE CASCADE for student_grades
        // (same as what you had in setup.sql: ON DELETE CASCADE)
        modelBuilder.Entity<StudentGrade>()
            .HasOne(sg => sg.Student)
            .WithMany(s => s.StudentGrades)
            .HasForeignKey(sg => sg.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
