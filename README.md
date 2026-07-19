# TaskManager Web API

A modern, high-performance ASP.NET Core 10 Web API serving as the backend for the Task Manager Application. It features user authentication, project categorization, database persistence, and an integrated Gemini AI service for parsing natural language phrases directly into structured tasks.

---

## Technologies

* **Framework:** .NET 10 (ASP.NET Core Web API)
* **Architecture:** 3-Layer Clean/Layered Architecture (Domain, Application, API)
* **ORM & Database:** Entity Framework Core 10 with PostgreSQL (Npgsql provider)
* **Authentication & Security:** JWT (JSON Web Tokens) Bearer Authorization
* **Generative AI:** Mscc.GenerativeAI SDK utilizing the `gemini-3.5-flash` model for task extraction
* **API Documentation:** OpenAPI (via `Microsoft.AspNetCore.OpenApi`)

---

## Features

### 🔐 Authentication
* `POST /api/auth/register` - Registers a new user.
* `POST /api/auth/login` - Authenticates a user and returns a JWT token.

### 📝 Task Management
* `GET /api/tasks` - Retrieves tasks for the authenticated user (supports filtering by completion status `status` and search query `search`).
* `GET /api/tasks/{id}` - Retrieves a specific task.
* `POST /api/tasks` - Creates a new task.
* `PUT /api/tasks/{id}` - Replaces all properties of an existing task.
* `PATCH /api/tasks/{id}` - Partially updates an existing task.
* `PATCH /api/tasks/{id}/complete` - Toggles the task's completion state.
* `DELETE /api/tasks/{id}` - Deletes a task.

### 📂 Project Management
* `GET /api/projects` - Retrieves all projects belonging to the authenticated user.
* `GET /api/projects/{id}` - Retrieves a specific project.
* `POST /api/projects` - Creates a new project.
* `PATCH /api/projects/{id}` - Renames/updates a project.
* `DELETE /api/projects/{id}` - Deletes a project.

### 🤖 AI Natural Language Task Creation
* `POST /api/nltask` - Parses a natural language sentence (e.g. *"Buy groceries tomorrow at 5 PM for Personal project"*) into structured JSON data and automatically creates and persists the task.

---

## The Process

### 1. Architecture & Design
The project is built on clean design principles, separating domain logic from external frameworks:
* **`TaskManager.Domain`**: Holds the core model definitions (`User`, `Project`, and `TaskItem`) with zero external dependencies.
* **`TaskManager.Application`**: Implements business services (`TaskService`, `ProjectService`, `AuthService`, `NLTaskService`), interfaces, and the database context (`ApplicationDbContext`).
* **`TaskManager.API`**: Exposes HTTP endpoints through controllers, manages DTO transformations, handles JWT validation, and configures Dependency Injection.

### 2. Database Integration & Automations
We used EF Core with PostgreSQL. The backend includes auto-migration logic in `Program.cs`. Every time the application starts up, it automatically applies any pending database migrations to the database, removing the need for manual migration deployment commands in staging or production.

### 3. AI-Driven Parsing Logic
The `NLTaskService` connects to the Gemini API using the `gemini-3.5-flash` model. It takes raw text inputs, resolves relative dates (like *"tomorrow"* or *"next Monday"*) dynamically against the current server date, identifies if a project should be associated, parses this into structured JSON, and saves the task.

---

## Running the project

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [PostgreSQL](https://www.postgresql.org/) database server running
* Gemini API Key (from [Google AI Studio](https://aistudio.google.com/))

### Configuration
Configure settings via environment variables or edit [appsettings.json](file:///c:/Users/crres/OneDrive/Desktop/SKILL%20DEV/DOTNET/TaskManagerApplication/TaskManager-api/TaskManager.API/appsettings.json):

1. **Database Connection:** Configure `ConnectionStrings:DefaultConnection` or set the `DATABASE_URL` environment variable (the API dynamically parses standard Heroku/Render-style PostgreSQL URLs on startup).
2. **JWT Secret Key:** Define a secure key in `Jwt:Secret` or set the `JWT_SECRET` environment variable.
3. **Gemini API Access:** Define your API key in `Gemini:ApiKey` or set the `GEMINI_API_KEY` environment variable.

### Running Commands
From the `TaskManager-api` directory:

1. **Build the solution:**
   ```powershell
   dotnet build TaskManager.slnx
   ```
2. **Run the API:**
   ```powershell
   dotnet run --project TaskManager.API/TaskManager.API.csproj
   ```
The API server will start. By default, it runs on HTTP port `5029` (`http://localhost:5029`) and HTTPS port `7081` (`https://localhost:7081`).
