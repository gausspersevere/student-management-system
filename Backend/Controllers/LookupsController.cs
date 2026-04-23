// ============================================================
// Controllers/LookupsController.cs
//
// Provides the dropdown data for the frontend:
//   GET /api/lookups/courses   → replaces: SELECT * FROM course
//   GET /api/lookups/sections  → replaces: SELECT * FROM section
//   GET /api/lookups/genders   → replaces: SELECT * FROM gender
//   GET /api/lookups/subjects  → replaces: SELECT * FROM subject
//
// In PHP these were scattered queries inside add_student.php
// and edit_student.php. Here they live in one dedicated controller.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentMS.Api.Data;
using StudentMS.Api.DTOs;

namespace StudentMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]   // → /api/lookups
public class LookupsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public LookupsController(ApplicationDbContext db) => _db = db;

    [HttpGet("courses")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetCourses()
    {
        var courses = await _db.Courses
            .OrderBy(c => c.CourseName)
            .Select(c => new LookupDto { Id = c.CourseId, Name = c.CourseName })
            .ToListAsync();
        return Ok(courses);
    }

    [HttpGet("sections")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetSections()
    {
        var sections = await _db.Sections
            .OrderBy(s => s.SectionName)
            .Select(s => new LookupDto { Id = s.SectionId, Name = s.SectionName })
            .ToListAsync();
        return Ok(sections);
    }

    [HttpGet("genders")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetGenders()
    {
        var genders = await _db.Genders
            .OrderBy(g => g.GenderName)
            .Select(g => new LookupDto { Id = g.GenderId, Name = g.GenderName })
            .ToListAsync();
        return Ok(genders);
    }

    [HttpGet("subjects")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetSubjects()
    {
        var subjects = await _db.Subjects
            .OrderBy(s => s.SubjectName)
            .Select(s => new LookupDto { Id = s.SubjectId, Name = s.SubjectName })
            .ToListAsync();
        return Ok(subjects);
    }

    // GET /api/lookups/stats  — dashboard summary numbers
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalStudents = await _db.Students.CountAsync();
        var totalCourses  = await _db.Courses.CountAsync();
        var totalSubjects = await _db.Subjects.CountAsync();
        var avgGrade      = await _db.StudentGrades.AverageAsync(g => (double?)g.Grade);

        return Ok(new
        {
            totalStudents,
            totalCourses,
            totalSubjects,
            averageGrade = avgGrade.HasValue ? Math.Round(avgGrade.Value, 2) : (double?)null,
        });
    }
}
