# 🎓 Student Information Management System (SIMS)

## 📌 Project Overview

This project is a **Student Information Management System (SIMS)** that performs full **CRUD operations** — Create, Read, Update, and Delete — for managing student records including their courses, sections, genders, subjects, and grades.


- **Backend:** ASP.NET Core 8 Web API
- **Frontend:** Vanilla HTML/CSS/JavaScript (Single Page Application)
- **Database:** MySQL hosted on Aiven Cloud
- **Deployment:** Render (API) + GitHub Pages (Frontend)


---

## 🌐 Live Demo

| Layer | Technology | URL |
|---|---|---|
| 🖥️ Frontend | GitHub Pages | [View Frontend](https://gausspersevere.github.io/student-management-system/) |
| ⚙️ Backend API | Render + ASP.NET Core | [View API](https://student-management-system-h4mm.onrender.com) |
| 📋 API Docs | Swagger UI | [View Swagger](https://student-management-system-h4mm.onrender.com/swagger) |

> ⚠️ **Note:** The backend is hosted on Render's free tier which sleeps after 15 minutes of inactivity. The first request may take 30–60 seconds to wake up.

---

## ✨ Features

- ✅ **Full CRUD** for student records
- ✅ **Relational data** — courses, sections, genders, subjects
- ✅ **Grade management** — assign and update subject grades per student
- ✅ **Average grade computation** per student
- ✅ **Sortable columns** — sort by name, age, or average grade
- ✅ **Filter by course and gender**
- ✅ **Live search** — filter students by name without page reload
- ✅ **Dashboard statistics** — total students, courses, subjects, global average grade
- ✅ **Responsive design** — works on desktop, tablet, and mobile
- ✅ **REST API** with full Swagger documentation
- ✅ **Cloud deployed** — accessible from anywhere

---


## 📁 Project Structure

```
StudentMS/
│
├── 📄 Dockerfile                    # Docker configuration for Render
├── 📄 render.yaml                   # Render deployment configuration
├── 📄 README.md                     # This file
├── 📄 .gitignore                    # Git ignore rules
│
├── 📂 Backend/                      # ASP.NET Core Web API
│   ├── 📄 StudentMS.Api.csproj      # Project & package dependencies
│   ├── 📄 Program.cs                # App entry point, middleware, DI config
│   ├── 📄 appsettings.json          # Local connection string config
│   │
│   ├── 📂 Controllers/
│   │   ├── 📄 StudentsController.cs # CRUD endpoints for students
│   │   └── 📄 LookupsController.cs  # Dropdown data endpoints + stats
│   │
│   ├── 📂 Models/
│   │   ├── 📄 Student.cs            # Student entity (maps to students table)
│   │   └── 📄 LookupModels.cs       # Course, Section, Gender, Subject, Grade
│   │
│   ├── 📂 DTOs/
│   │   └── 📄 StudentDtos.cs        # Request/response data transfer objects
│   │
│   └── 📂 Data/
│       ├── 📄 ApplicationDbContext.cs  # EF Core database context
│       └── 📄 DbDataSeeder.cs          # Initial data seeder
│
└── 📂 Frontend/
    └── 📄 index.html                # Single-page frontend application
```
---

## 📚 What I Learned

This project involved learning and applying the following concepts from scratch:

- Setting up and configuring an **ASP.NET Core Web API** project
- Using **Entity Framework Core** with MySQL instead of raw SQL queries
- Understanding the **request/response lifecycle** in a REST API
- Writing **LINQ queries** as a replacement for hand-written SQL
- Designing and consuming a **JSON API** from a JavaScript frontend
- Using **Git and GitHub** for version control and collaboration
- Deploying a **.NET application to Render** using Docker
- Hosting a **static frontend on GitHub Pages**
- Managing a **cloud MySQL database on Aiven**
- Configuring **environment variables** for secure credential management

---

