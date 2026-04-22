// ============================================================
// DTOs/StudentDtos.cs
//
// DTOs (Data Transfer Objects) control what data flows in/out
// of your API endpoints. In PHP, you had no DTOs — the raw
// database row was mixed directly into HTML. Here we separate
// the database shape from what the API exposes.
// ============================================================

namespace StudentMS.Api.DTOs;

// ---- What the frontend SENDS when creating/updating a student ----
public class StudentCreateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Birthdate { get; set; } = string.Empty;   // "YYYY-MM-DD"
    public int CourseId { get; set; }
    public int SectionId { get; set; }
    public int GenderId { get; set; }
}

// ---- What the API RETURNS when reading a student ----
public class StudentResponseDto
{
    public int StudentId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Birthdate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string GenderName { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int SectionId { get; set; }
    public int GenderId { get; set; }
    public List<GradeDto> Grades { get; set; } = new();
    public decimal? AverageGrade { get; set; }
}

public class GradeDto
{
    public int GradeId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal Grade { get; set; }
}

// ---- Lookup DTOs for dropdowns (course, section, gender, subject) ----
public class LookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// ---- What the frontend sends to assign a grade ----
public class AssignGradeDto
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public decimal Grade { get; set; }
}
