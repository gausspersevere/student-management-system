# 🎓 Student Information Management System
## PHP → ASP.NET Core Web API Refactoring Guide

---

## 📁 Folder Structure

```
StudentMS/
│
├── render.yaml                   ← Render deployment config
│
├── Backend/                      ← ASP.NET Core Web API
│   ├── StudentMS.Api.csproj      ← Project file (like composer.json in PHP)
│   ├── Program.cs                ← App entry point + config (replaces config.php)
│   ├── appsettings.json          ← Connection strings & settings
│   │
│   ├── Models/                   ← Database table definitions (C# classes)
│   │   ├── Student.cs            ← students table
│   │   └── LookupModels.cs       ← course, section, gender, subject, student_grades
│   │
│   ├── DTOs/                     ← Data Transfer Objects (API request/response shapes)
│   │   └── StudentDtos.cs
│   │
│   ├── Data/                     ← Database layer
│   │   ├── ApplicationDbContext.cs  ← EF Core "database manager" (replaces mysqli_connect)
│   │   └── DbDataSeeder.cs          ← Initial data seeder (replaces setup.sql INSERT INTO)
│   │
│   └── Controllers/              ← API endpoints (replaces PHP pages)
│       ├── StudentsController.cs ← /api/students (CRUD for students)
│       └── LookupsController.cs  ← /api/lookups  (courses, sections, genders, subjects)
│
└── Frontend/
    └── index.html                ← Single-page app that calls the API with fetch()
```

---

## 🔄 PHP → ASP.NET Core Mapping

| PHP File | ASP.NET Core Equivalent |
|---|---|
| `config.php` | `Program.cs` + `appsettings.json` |
| `index.php` | `GET /api/students` in `StudentsController` |
| `add_student.php` | `POST /api/students` in `StudentsController` |
| `edit_student.php` | `PUT /api/students/{id}` in `StudentsController` |
| `delete.php` | `DELETE /api/students/{id}` in `StudentsController` |
| `setup.sql` | EF Core Migrations + `DbDataSeeder.cs` |
| `mysqli_connect()` | `ApplicationDbContext` + EF Core |
| `mysqli_prepare()` + `bind_param()` | LINQ queries (`.Where()`, `.Include()`) |
| `htmlspecialchars()` | Automatic in Razor / JSON serialization |
| PHP `$_POST` | `[FromBody] StudentCreateDto dto` |
| PHP `$_GET` | `[FromQuery] int? courseId` |

---

## 🚀 Quick Start — Local Development

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL (XAMPP is fine)
- Any browser

### Step 1 — Set up MySQL
Your **existing `setup.sql`** still works! Import it in phpMyAdmin as before.

### Step 2 — Configure the connection string
Edit `Backend/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=student_management_db;User=root;Password=;"
  }
}
```

### Step 3 — Run the API
```bash
cd Backend
dotnet run
```
The API starts at: http://localhost:5000
Swagger UI at:    http://localhost:5000/swagger

### Step 4 — Open the frontend
Open `Frontend/index.html` in your browser. It automatically connects to `http://localhost:5000/api`.

> ⚠️ If you get CORS errors, make sure `dotnet run` is running first.

---

## 🌐 API Endpoints Reference

### Students
```
GET    /api/students              → Get all students (with filters)
GET    /api/students/{id}         → Get one student
POST   /api/students              → Create student
PUT    /api/students/{id}         → Update student
DELETE /api/students/{id}         → Delete student
POST   /api/students/grades       → Assign/update a grade
```

### Lookups (for dropdowns)
```
GET    /api/lookups/courses       → All courses
GET    /api/lookups/sections      → All sections
GET    /api/lookups/genders       → All genders
GET    /api/lookups/subjects      → All subjects
GET    /api/lookups/stats         → Dashboard statistics
```

### Query Parameters for GET /api/students
```
?courseId=1        → Filter by course
?genderId=2        → Filter by gender
?sortBy=lastname   → Sort field (lastname | age | grade)
?order=DESC        → Sort direction (ASC | DESC)
```

### Example JSON Payloads

**POST /api/students** (Create):
```json
{
  "firstName": "Juan",
  "lastName": "dela Cruz",
  "birthdate": "2002-05-15",
  "courseId": 1,
  "sectionId": 2,
  "genderId": 1
}
```

**POST /api/students/grades** (Assign grade):
```json
{
  "studentId": 1,
  "subjectId": 3,
  "grade": 88.50
}
```

---

## ☁️ Deploying to Render

### Step 1 — Push to GitHub
```bash
git init
git add .
git commit -m "Initial ASP.NET Core SIMS"
git remote add origin https://github.com/YOUR_USERNAME/studentms.git
git push -u origin main
```

### Step 2 — Set up MySQL on Render (or use PlanetScale/Aiven)
Render doesn't host MySQL databases on free tier. Use one of these free alternatives:
- **[Aiven MySQL](https://aiven.io/)** — free 1-month trial, then $0 on hobbyist plan
- **[PlanetScale](https://planetscale.com/)** — free tier available
- **[Railway](https://railway.app/)** — MySQL free trial

Get your connection string from whichever provider you choose.

### Step 3 — Create a Web Service on Render
1. Go to [render.com](https://render.com) → New → Web Service
2. Connect your GitHub repository
3. Render will auto-detect the `render.yaml` file

### Step 4 — Set Environment Variables on Render
In Render Dashboard → Your Service → Environment:
```
ASPNETCORE_ENVIRONMENT = Production
ConnectionStrings__DefaultConnection = Server=YOUR_HOST;Port=3306;Database=YOUR_DB;User=YOUR_USER;Password=YOUR_PASS;
```

### Step 5 — Update Frontend API URL
Edit `Frontend/index.html`, change:
```javascript
const API_BASE = 'http://localhost:5000/api';
```
To your Render URL:
```javascript
const API_BASE = 'https://studentms-api.onrender.com/api';
```

Then host `index.html` on:
- [Render Static Site](https://render.com/docs/static-sites) (free)
- [GitHub Pages](https://pages.github.com/) (free)
- [Netlify](https://netlify.com/) (free)

---

## 🔑 Key Architecture Differences

### PHP (Old Way)
- One file = one page = HTML + SQL + business logic mixed together
- Server renders HTML and sends the whole page on each click
- Each page reload = new connection to MySQL
- No separation of concerns — config, queries, display all in one file

### ASP.NET Core Web API (New Way)
- **Separation of concerns**: Models (data), Controllers (logic), Frontend (display) are separate
- **API-first**: Backend only sends/receives JSON — no HTML generation
- **Stateless**: Each HTTP request is independent and self-contained
- **Type-safe**: C# models catch errors at compile time, not runtime
- **Async**: All database operations are `await`-ed — non-blocking
- **Dependency Injection**: Services are configured once in `Program.cs` and shared throughout

### Database Layer Comparison
```
PHP:                              ASP.NET Core:
------                            -------------
mysqli_connect()            →     DbContext + EF Core
mysqli_prepare()            →     LINQ: _db.Students.Where(...)
mysqli_stmt_bind_param()    →     Strongly-typed C# objects
mysqli_stmt_execute()       →     await _db.SaveChangesAsync()
mysqli_fetch_assoc()        →     .ToListAsync() returns C# objects
Raw SQL strings             →     Type-safe LINQ expressions
```

---

## 🛠️ Useful Commands

```bash
# Run the API
dotnet run

# Add a new EF Core migration (when you change models)
dotnet ef migrations add YourMigrationName

# Apply migrations to database
dotnet ef database update

# Build for production
dotnet publish -c Release -o ./publish

# Restore packages (like npm install / composer install)
dotnet restore
```

---

## 📚 Technologies Used

| Technology | Purpose | PHP Equivalent |
|---|---|---|
| ASP.NET Core 8 | Web framework | PHP + Apache |
| Entity Framework Core | ORM (database) | mysqli functions |
| Pomelo.EFCore.MySql | MySQL driver | mysqli extension |
| Swashbuckle | Swagger API docs | None (PHP had none) |
| HTML + Vanilla JS | Frontend SPA | PHP-rendered HTML |
| Fetch API | HTTP calls to backend | Form `<action>` submits |
