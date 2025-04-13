TODO API - .NET 7 Minimal Web API with JWT Authentication
==========================================================

Overview
--------
This is a simple Todo Web API built with ASP.NET Core (.NET 7), featuring:
- JWT Authentication
- Todo CRUD operations
- User registration and login
- Refresh token generation
- Exception handling with custom exceptions
- Logging
- Validation using FluentValidation
- Swagger UI for API testing
- Unit testing using xUnit and FluentAssertions

Project Structure
-----------------
- Controllers/
  - Handles incoming API requests (TodoController, AuthController)
  
- Services/
  - Business logic lives here
  - JwtTokenService.cs
  - TodoService.cs
  - UserService.cs

- DTOs/
  - Data Transfer Objects for requests and responses

- Models/
  - Database entities: User, Todo

- Data/
  - AppDbContext.cs (EF Core context)

- Middleware/
  - Custom JWT middleware and global error handling

- Exceptions/
  - Custom exception classes (NotFoundException, ValidationException, etc.)

- Tests/
  - Unit tests for services using xUnit


How to Run
----------
1. Clone the repository.
2. Open in Visual Studio or use `dotnet` CLI.
3. Setup your `appsettings.json` with your JWT key and issuer:

"Jwt": { "Key": "your_super_secret_key_here", "Issuer": "your_issuer", "Audience": "your_audience", "ExpiresInMinutes": "60" }

4. Run migrations (if using real DB) or just run the project:

dotnet run

5. Open Swagger UI at:

https://localhost:<port>/swagger



Testing
-------
Tests are located in the `TodoApp.Tests` project.
To run tests:

dotnet test


Tech Stack
----------
- .NET 7
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger
- JWT
- xUnit (for testing)

Author
------
Developed by Kel Xin


