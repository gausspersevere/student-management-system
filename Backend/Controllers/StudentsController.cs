// ============================================================
// Controllers/StudentsController.cs
//
// PHP EQUIVALENT MAPPING:
//   index.php (SELECT + JOINs)   → GET /api/students
//   add_student.php (INSERT)     → POST /api/students
//   edit_student.php (UPDATE)    → PUT /api/students/{id}
//   delete.php (DELETE)          → DELETE /api/students/{id}
//
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMS.Api.Data;
using StudentMS.Api.DTOs;
using StudentMS.Api.Models;

namespace StudentMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]   // → /api/students
public class StudentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    // Dependency Injection: EF Core context is auto-provided
    // In PHP you did: include 'config.php'; to get $conn
    public StudentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // --------------------------------------------------------
    // GET /api/students
    // Equivalent to: index.php — SELECT with LEFT JOINs
    // Optional query params: ?courseId=1&genderId=2
    // --------------------------------------------------------
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetAll(
        [FromQuery] int? courseId,
        [FromQuery] int? genderId,
        [FromQuery] string? sortBy,
        [FromQuery] string? order)
    {
        var query = _db.Students
            .Include(s => s.Course)
            .Include(s => s.Section)
            .Include(s => s.Gender)
            .Include(s => s.StudentGrades)
                .ThenInclude(sg => sg.Subject)
            .AsQueryable();

        // Filtering (equivalent to your WHERE clause in index.php)
        if (courseId.HasValue && courseId > 0)
            query = query.Where(s => s.CourseId == courseId);
        if (genderId.HasValue && genderId > 0)
            query = query.Where(s => s.GenderId == genderId);

        // Sorting (equivalent to your ORDER BY logic in index.php)
        bool descending = order?.ToUpper() == "DESC";
        query = sortBy?.ToLower() switch
        {
            "lastname"  => descending ? query.OrderByDescending(s => s.LastName)  : query.OrderBy(s => s.LastName),
            "age"       => descending ? query.OrderBy(s => s.Birthdate)           : query.OrderByDescending(s => s.Birthdate),
            "grade"     => descending
                            ? query.OrderByDescending(s => s.StudentGrades.Average(g => (double?)g.Grade))
                            : query.OrderBy(s => s.StudentGrades.Average(g => (double?)g.Grade)),
            _           => query.OrderBy(s => s.FirstName),
        };

        var students = await query.ToListAsync();

        // Map to DTOs — this is where we shape the JSON response
        var result = students.Select(s => MapToDto(s)).ToList();
        return Ok(result);
    }

    // --------------------------------------------------------
    // GET /api/students/{id}
    // Get a single student by ID
    // --------------------------------------------------------
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponseDto>> GetById(int id)
    {
        var student = await _db.Students
            .Include(s => s.Course)
            .Include(s => s.Section)
            .Include(s => s.Gender)
            .Include(s => s.StudentGrades)
                .ThenInclude(sg => sg.Subject)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound(new { message = $"Student with ID {id} not found." });

        return Ok(MapToDto(student));
    }

    // --------------------------------------------------------
    // POST /api/students
    // Equivalent to: add_student.php POST handler (INSERT INTO)
    // Body: { "firstName": "Juan", "lastName": "dela Cruz", ... }
    // --------------------------------------------------------
    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> Create([FromBody] StudentCreateDto dto)
    {
        // Validation (equivalent to your $errors[] checks in add_student.php)
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!DateOnly.TryParse(dto.Birthdate, out var birthdate))
            return BadRequest(new { message = "Invalid birthdate format. Use YYYY-MM-DD." });

        // Verify foreign keys exist (course, section, gender)
        if (!await _db.Courses.AnyAsync(c => c.CourseId == dto.CourseId))
            return BadRequest(new { message = "Invalid course selected." });
        if (!await _db.Sections.AnyAsync(s => s.SectionId == dto.SectionId))
            return BadRequest(new { message = "Invalid section selected." });
        if (!await _db.Genders.AnyAsync(g => g.GenderId == dto.GenderId))
            return BadRequest(new { message = "Invalid gender selected." });

        var student = new Student
        {
            FirstName  = dto.FirstName.Trim(),
            LastName   = dto.LastName.Trim(),
            Birthdate  = birthdate,
            CourseId   = dto.CourseId,
            SectionId  = dto.SectionId,
            GenderId   = dto.GenderId,
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();  // ← equivalent to mysqli_stmt_execute()

        // Reload with related data for the response
        await _db.Entry(student).Reference(s => s.Course).LoadAsync();
        await _db.Entry(student).Reference(s => s.Section).LoadAsync();
        await _db.Entry(student).Reference(s => s.Gender).LoadAsync();

        // 201 Created + location header pointing to the new resource
        return CreatedAtAction(nameof(GetById), new { id = student.StudentId }, MapToDto(student));
    }

    // --------------------------------------------------------
    // PUT /api/students/{id}
    // Equivalent to: edit_student.php POST handler (UPDATE SET)
    // --------------------------------------------------------
    [HttpPut("{id}")]
    public async Task<ActionResult<StudentResponseDto>> Update(int id, [FromBody] StudentCreateDto dto)
    {
        var student = await _db.Students
            .Include(s => s.Course)
            .Include(s => s.Section)
            .Include(s => s.Gender)
            .Include(s => s.StudentGrades)
                .ThenInclude(sg => sg.Subject)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
            return NotFound(new { message = $"Student with ID {id} not found." });

        if (!DateOnly.TryParse(dto.Birthdate, out var birthdate))
            return BadRequest(new { message = "Invalid birthdate format. Use YYYY-MM-DD." });

        // Update fields — equivalent to your UPDATE students SET ... WHERE student_id=?
        student.FirstName  = dto.FirstName.Trim();
        student.LastName   = dto.LastName.Trim();
        student.Birthdate  = birthdate;
        student.CourseId   = dto.CourseId;
        student.SectionId  = dto.SectionId;
        student.GenderId   = dto.GenderId;

        await _db.SaveChangesAsync();

        return Ok(MapToDto(student));
    }

    // --------------------------------------------------------
    // DELETE /api/students/{id}
    // Equivalent to: delete.php (DELETE FROM students WHERE ...)
    // Cascade delete on student_grades is handled by EF Core
    // (same as your ON DELETE CASCADE in setup.sql)
    // --------------------------------------------------------
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var student = await _db.Students.FindAsync(id);

        if (student == null)
            return NotFound(new { message = $"Student with ID {id} not found." });

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        return NoContent();   // 204 — success with no response body
    }

    // --------------------------------------------------------
    // POST /api/students/grades
    // Assign or update a grade for a student (add_grade.php)
    // --------------------------------------------------------
    [HttpPost("grades")]
    public async Task<IActionResult> AssignGrade([FromBody] AssignGradeDto dto)
    {
        if (dto.Grade < 0 || dto.Grade > 100)
            return BadRequest(new { message = "Grade must be between 0 and 100." });

        // Check if grade already exists → update it (upsert pattern)
        var existing = await _db.StudentGrades
            .FirstOrDefaultAsync(sg => sg.StudentId == dto.StudentId && sg.SubjectId == dto.SubjectId);

        if (existing != null)
        {
            existing.Grade = dto.Grade;
        }
        else
        {
            _db.StudentGrades.Add(new StudentGrade
            {
                StudentId = dto.StudentId,
                SubjectId = dto.SubjectId,
                Grade     = dto.Grade,
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Grade saved successfully." });
    }

    // --------------------------------------------------------
    // Private helper: convert Student entity → StudentResponseDto
    // This is what gets serialized to JSON and sent to the frontend
    // --------------------------------------------------------
    private static StudentResponseDto MapToDto(Student s)
    {
        var today    = DateOnly.FromDateTime(DateTime.Today);
        var age      = today.Year - s.Birthdate.Year;
        if (s.Birthdate > today.AddYears(-age)) age--;  // handle birthdays later in year

        var grades = s.StudentGrades.Select(sg => new GradeDto
        {
            GradeId     = sg.GradeId,
            SubjectName = sg.Subject?.SubjectName ?? "Unknown",
            Grade       = sg.Grade,
        }).ToList();

        return new StudentResponseDto
        {
            StudentId   = s.StudentId,
            FirstName   = s.FirstName,
            LastName    = s.LastName,
            Birthdate   = s.Birthdate.ToString("yyyy-MM-dd"),
            Age         = age,
            CourseName  = s.Course?.CourseName ?? "",
            SectionName = s.Section?.SectionName ?? "",
            GenderName  = s.Gender?.GenderName ?? "",
            CourseId    = s.CourseId,
            SectionId   = s.SectionId,
            GenderId    = s.GenderId,
            Grades      = grades,
            AverageGrade = grades.Count > 0 ? Math.Round(grades.Average(g => g.Grade), 2) : null,
        };
    }
}
