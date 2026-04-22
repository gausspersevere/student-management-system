// ============================================================
// Models/Course.cs  — Maps to the `course` table
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMS.Api.Models;

public class Course
{
    [Key]
    [Column("course_id")]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("course_name")]
    public string CourseName { get; set; } = string.Empty;

    // One course has many students
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

// ============================================================
// Models/Section.cs  — Maps to the `section` table
// ============================================================
public class Section
{
    [Key]
    [Column("section_id")]
    public int SectionId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("section_name")]
    public string SectionName { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}

// ============================================================
// Models/Gender.cs  — Maps to the `gender` table
// ============================================================
public class Gender
{
    [Key]
    [Column("gender_id")]
    public int GenderId { get; set; }

    [Required]
    [MaxLength(20)]
    [Column("gender_name")]
    public string GenderName { get; set; } = string.Empty;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}

// ============================================================
// Models/Subject.cs  — Maps to the `subject` table
// ============================================================
public class Subject
{
    [Key]
    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("subject_name")]
    public string SubjectName { get; set; } = string.Empty;

    public ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}

// ============================================================
// Models/StudentGrade.cs  — Maps to the `student_grades` table
// ============================================================
public class StudentGrade
{
    [Key]
    [Column("grade_id")]
    public int GradeId { get; set; }

    [Column("student_id")]
    public int StudentId { get; set; }

    [Column("subject_id")]
    public int SubjectId { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Grade { get; set; }

    // Navigation
    public Student? Student { get; set; }
    public Subject? Subject { get; set; }
}
