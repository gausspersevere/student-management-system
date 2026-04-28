

namespace StudentMS.Api.Data;

public static class DbDataSeeder
{
    /// <summary>
    /// Seeds the database with initial lookup data.
    /// Call this from Program.cs after migration.
    /// Equivalent to the INSERT INTO blocks in your setup.sql
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Only seed if tables are empty
        if (!db.Courses.Any())
        {
            db.Courses.AddRange(
                new Models.Course { CourseName = "Bachelor of Science in Information Technology" },
                new Models.Course { CourseName = "Bachelor of Science in Computer Science" },
                new Models.Course { CourseName = "Bachelor of Science in Education" },
                new Models.Course { CourseName = "Bachelor of Science in Nursing" },
                new Models.Course { CourseName = "Bachelor of Science in Business Administration" }
            );
        }

        if (!db.Sections.Any())
        {
            db.Sections.AddRange(
                new Models.Section { SectionName = "Section A" },
                new Models.Section { SectionName = "Section B" },
                new Models.Section { SectionName = "Section C" },
                new Models.Section { SectionName = "Section D" }
            );
        }

        if (!db.Genders.Any())
        {
            db.Genders.AddRange(
                new Models.Gender { GenderName = "Male" },
                new Models.Gender { GenderName = "Female" },
                new Models.Gender { GenderName = "Other" }
            );
        }

        if (!db.Subjects.Any())
        {
            db.Subjects.AddRange(
                new Models.Subject { SubjectName = "Mathematics" },
                new Models.Subject { SubjectName = "English" },
                new Models.Subject { SubjectName = "Science" },
                new Models.Subject { SubjectName = "Filipino" },
                new Models.Subject { SubjectName = "Physical Education" },
                new Models.Subject { SubjectName = "Information Technology" },
                new Models.Subject { SubjectName = "Programming Fundamentals" }
            );
        }

        await db.SaveChangesAsync();
    }
}
