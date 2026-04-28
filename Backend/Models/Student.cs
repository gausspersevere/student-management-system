

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMS.Api.Models;

public class Student
{
    [Key]
    [Column("student_id")]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("birthdate")]
    public DateOnly Birthdate { get; set; }

    // Foreign Keys — equivalent to course_id, section_id, gender_id columns
    [Column("course_id")]
    public int CourseId { get; set; }

    [Column("section_id")]
    public int SectionId { get; set; }

    [Column("gender_id")]
    public int GenderId { get; set; }

    // Navigation Properties — EF Core uses these to do JOINs automatically
    // In PHP you wrote JOIN manually in SQL strings
    public Course? Course { get; set; }
    public Section? Section { get; set; }
    public Gender? Gender { get; set; }
    public ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
