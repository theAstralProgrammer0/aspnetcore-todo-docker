# Running an ASP.NET Core Web API and SQLite Database in Docker Containers

## Goal
Build and run a full-fledged ASP.NET Core Web API backend for a ToDo App using Docker containers, with SQLite as the database. The approach ensures:
- Persistent data and configurations across container restarts.
- A fully automated setup using scripts, Dockerfiles, and Docker Compose.
- No local installations apart from Docker-related tools.
- Development via CLI only, using Vim as the text editor.

## Steps to Achieve the Goal

### Step 1: Set Up the Project Structure
1. **Create the Project Directory:**
   ```bash
   mkdir aspnetcore-todo-docker && cd aspnetcore-todo-docker
   ```

2. **Generate the ASP.NET Core Web API Project:**
   ```bash
   docker run --rm -v $(pwd):/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 \
     dotnet new webapi -o TodoApp
   ```

3. **Navigate to the Project Directory:**
   ```bash
   cd TodoApp
   ```

### Step 2: Define the ToDo Model and Context
1. **Create the `TodoItem` Model:**
   Create a `Models` directory and a `TodoItem.cs` file:
   ```bash
   mkdir Models && vim Models/TodoItem.cs
   ```
   Add the following code:
   ```csharp
   namespace TodoApp.Models
   {
       public class TodoItem
       {
           public int Id { get; set; }
           public string Name { get; set; }
           public bool IsComplete { get; set; }
       }
   }
   ```

2. **Create the `TodoContext` Class:**
   ```bash
   vim Models/TodoContext.cs
   ```
   Add:
   ```csharp
   using Microsoft.EntityFrameworkCore;

   namespace TodoApp.Models
   {
       public class TodoContext : DbContext
       {
           public TodoContext(DbContextOptions<TodoContext> options) : base(options) { }

           public DbSet<TodoItem> TodoItems { get; set; }
       }
   }
   ```

### Step 3: Configure SQLite in ASP.NET Core
1. **Install Entity Framework Core SQLite Provider:**
   Add the package via Docker:
   ```bash
   docker run --rm -v $(pwd):/app -w /app mcr.microsoft.com/dotnet/sdk:9.0 \
     dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   ```

2. **Update `appsettings.json` to Use SQLite:**
   ```bash
   vim appsettings.json
   ```
   Add:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=Todo.db"
     }
   }
   ```

3. **Register the Database Context:**
   Update `Program.cs`:
   ```csharp
   using Microsoft.EntityFrameworkCore;
   using TodoApp.Models;

   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container.
   builder.Services.AddControllers();
   builder.Services.AddDbContext<TodoContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

   var app = builder.Build();

   app.MapControllers();

   app.Run();
   ```

### Step 4: Create the Todo Controller
1. **Add the Controller:**
   ```bash
   mkdir Controllers && vim Controllers/TodoController.cs
   ```
   Add:
   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using TodoApp.Models;
   using System.Linq;

   namespace TodoApp.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class TodoController : ControllerBase
       {
           private readonly TodoContext _context;

           public TodoController(TodoContext context)
           {
               _context = context;
           }

           [HttpGet]
           public IActionResult GetAll() => Ok(_context.TodoItems.ToList());

           [HttpPost]
           public IActionResult Create(TodoItem item)
           {
               _context.TodoItems.Add(item);
               _context.SaveChanges();
               return CreatedAtAction(nameof(GetAll), new { id = item.Id }, item);
           }
       }
   }
   ```

### Step 5: Prepare Docker Infrastructure
1. **Create the Dockerfile:**
   ```bash
   vim Dockerfile
   ```
   Add:
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
   WORKDIR /app
   COPY . .
   RUN dotnet restore
   RUN dotnet publish -c Release -o /out

   FROM mcr.microsoft.com/dotnet/aspnet:9.0
   WORKDIR /app
   COPY --from=build /out .
   ENTRYPOINT ["dotnet", "TodoApp.dll"]
   ```

2. **Create the Docker-Compose File:**
   ```bash
   vim docker-compose.yml
   ```
   Add:
   ```yaml
   version: '3.8'
   services:
     todoapi:
       build: .
       ports:
         - "5000:5000"
       volumes:
         - ./data:/app/data
       environment:
         - ASPNETCORE_ENVIRONMENT=Development
       depends_on:
         - db

     db:
       image: sqlite
       volumes:
         - ./data:/data
   ```

3. **Run the Containers:**
   ```bash
   docker-compose up --build
   ```

### Step 6: Test and Validate
1. **Test the API:**
   Use `curl` or Postman to interact with `http://localhost:5000/api/todo`.

2. **Validate Persistence:**
   Stop and restart the containers to confirm data persistence.

### Step 7: Automate Setup
1. **Create a Setup Script:**
   ```bash
   vim setup.sh
   ```
   Add:
   ```bash
   #!/bin/bash
   set -e

   docker-compose down
   docker-compose up --build
   ```

2. **Run the Script:**
   ```bash
   chmod +x setup.sh
   ./setup.sh
   ```

### Conclusion
This setup enables a lightweight, CLI-based development environment for your ASP.NET Core Web API and SQLite database. All configurations and data persist across container restarts, ensuring a smooth and efficient development workflow.


