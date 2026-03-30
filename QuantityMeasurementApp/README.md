# QuantityMeasurementApp

QuantityMeasurementApp is a .NET 8 layered application for secure quantity operations across Length, Weight, Volume, and Temperature.

It includes:
- JWT-based authentication
- Rate-limited APIs
- Unit conversion and arithmetic operations
- SQL Server persistence
- Redis-backed history caching
- SQL audit trigger for history table changes
- Static frontend pages for auth + operation dashboard

## Table of Contents

1. Project Overview
2. Technology Stack
3. Solution Structure
4. Architecture and Layer Responsibilities
5. Dependency Injection and Composition
6. HTTP Middleware Pipeline and Flow
7. API Reference
8. DTOs, Entities, and Domain Models
9. Quantity Domain Logic and Unit System
10. Persistence, Caching, and Audit
11. Security Design
12. Frontend Design and API Integration
13. Setup and Run
14. Migrations and Database Commands
15. Testing
16. Key Classes and Major Functions
17. Known Constraints and Suggested Improvements

## 1) Project Overview

The application solves quantity-related use cases with clear separation of concerns:
- API layer handles routing, middleware, authorization, and HTTP contracts.
- Business layer handles use-case orchestration (convert/add/subtract/divide/auth/history).
- Infrastructure layer handles EF Core persistence and Redis caching.
- Model layer defines domain abstractions, concrete units, DTOs, entities, and interfaces.
- Test layer validates quantity behavior across use cases.

Main functional areas:
- Register/Login and JWT token issuance
- Quantity conversion between compatible units
- Addition/subtraction within same category
- Divide quantity by scalar
- Divide quantity by quantity (unitless scalar)
- Operation history read and clear

## 2) Technology Stack

- .NET 8
- ASP.NET Core Web API
- C#
- Entity Framework Core (SQL Server)
- SQL Server
- Redis via IDistributedCache
- JWT Bearer Authentication
- Swashbuckle / Swagger OpenAPI
- NUnit test framework
- Static HTML/CSS/JavaScript frontend served from wwwroot

Main packages used:
- DotNetEnv
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Extensions.Caching.StackExchangeRedis
- Swashbuckle.AspNetCore
- NUnit + NUnit3TestAdapter

## 3) Solution Structure

- QuantityMeasurement.API
  - Program startup and middleware composition
  - Controllers: AuthController, QuantityController, HistoryController
  - Middleware: GlobalException, TimeLogging
  - Static frontend in wwwroot
- QuantityMeasurement.BusinessLayer
  - Interfaces and service implementations
  - DI extension method registration
- QuantityMeasurement.Infrastructure
  - QuantityDbContext
  - Repositories (HistoryRepository)
  - EF migrations (including audit trigger migration)
- QuantityMeasurement.ModelLayer
  - Core quantity abstractions and Unit metadata
  - Category abstractions and concrete unit classes
  - DTOs, entities, and contracts
- QuantityMeasurement.Tests
  - UC-style NUnit tests for quantity behavior

## 4) Architecture and Layer Responsibilities

### 4.1 Layered Architecture

The project follows an N-tier layered architecture:

1. Presentation/API Layer
- Endpoint definitions and route handling
- Authorization attributes and HTTP responses
- Middleware-based cross-cutting concerns

2. Application/Business Layer
- Use-case logic for operations and auth
- Input validation and orchestration
- History recording coordination

3. Infrastructure Layer
- Data persistence via EF Core
- History data access via repository pattern
- Distributed cache read-through / invalidation logic

4. Domain/Model Layer
- Unit metadata and conversion formulas
- Category-safe quantity operations
- Data contracts (DTO/entity/interface)

### 4.2 Dependency Direction

- API depends on BusinessLayer interfaces.
- BusinessLayer depends on Infrastructure interfaces and Model types.
- Infrastructure depends on Model entities and contracts.
- ModelLayer is independent of higher layers.

## 5) Dependency Injection and Composition

Service registration is centralized in:
- QuantityMeasurement.BusinessLayer/DependencyInjection/ServiceCollectionExtensions.cs

Registered services:
- IQuantityService -> QuantityService
- IAuthService -> AuthService
- IPasswordHasher -> PasswordHasher
- IHistoryService -> HistoryService
- IHistoryRepository -> HistoryRepository
- IQuantityDbContext -> QuantityDbContext

Data and cache registration:
- AddDbContext<QuantityDbContext> using ConnectionStrings:DefaultConnection
- AddStackExchangeRedisCache using Redis:Configuration and Redis:InstanceName
- AddMemoryCache

## 6) HTTP Middleware Pipeline and Flow

Configured in Program.cs.

Pipeline order:
1. Swagger (Development only)
2. UseDefaultFiles
3. UseStaticFiles
4. UseGlobalException
5. UseTimeLogging
6. UseHttpsRedirection
7. UseCors("DevCors")
8. UseRateLimiter
9. UseAuthentication
10. UseAuthorization
11. MapControllers

### 6.1 Cross-Cutting Behavior

- GlobalException middleware maps common exceptions to ProblemDetails responses.
- TimeLogging middleware logs request execution time.
- Fixed-window rate limiting policy fixedWindowLimiter is enabled on controllers.

Current rate limiter settings:
- Partition key: remote IP
- PermitLimit = 10
- Window = 100 seconds
- QueueLimit = 3
- QueueProcessingOrder = OldestFirst
- Rejection status code = 429

### 6.2 End-to-End Request Flow

Typical request path:
1. Client sends HTTP request.
2. ASP.NET middleware pipeline processes request.
3. Authentication validates bearer token (when endpoint requires auth).
4. Controller action executes.
5. Controller delegates to business service.
6. Service performs domain logic and persistence calls.
7. Repository/DbContext executes DB and cache operations.
8. Controller returns HTTP response JSON.

Example convert flow:
1. POST /api/quantity/convert with QuantityRequestDto JSON.
2. QuantityController.Convert calls IQuantityService.Convert.
3. QuantityService resolves units, constructs Quantity object, converts result.
4. HistoryRepository.Save persists operation in Histories and invalidates cache.
5. API returns QuantityResultDto.

## 7) API Reference

Base pattern:
- api/[controller]

### 7.1 AuthController (AllowAnonymous)

1. POST /api/auth/register
- Parameters: username, password (query/form parameter binding in current implementation)
- Response: 200 OK with message text

2. POST /api/auth/login
- Parameters: username, password
- Response: 200 OK with JWT token string

### 7.2 QuantityController (Authorize + fixedWindowLimiter)

All endpoints require bearer token.

1. POST /api/quantity/convert
- Input: QuantityRequestDto with value1, unit1, targetUnit
- Response: QuantityResultDto

2. POST /api/quantity/add
- Input: QuantityRequestDto with value1, unit1, value2, unit2, optional targetUnit
- Response: QuantityResultDto

3. POST /api/quantity/subtract
- Input: QuantityRequestDto with value1, unit1, value2, unit2, optional targetUnit
- Response: QuantityResultDto

4. POST /api/quantity/divide-scalar
- Input: QuantityRequestDto with value1, unit1, scalar
- Response: QuantityResultDto

5. POST /api/quantity/divide-quantity
- Input: QuantityRequestDto with value1, unit1, value2, unit2
- Response: double scalar (unitless)

### 7.3 HistoryController (Authorize + fixedWindowLimiter)

1. GET /api/history
- Response: List<HistoryDto>

2. DELETE /api/history
- Clears all history records
- Response: 204 NoContent

### 7.4 Error Response Model

Errors are returned by GlobalException middleware as application/problem+json with fields such as:
- status
- title
- detail
- instance
- traceId (extension)
- errors (extension, for mapped validation/input cases)

## 8) DTOs, Entities, and Domain Models

### 8.1 DTOs

- QuantityRequestDto
  - value1, unit1, value2, unit2, targetUnit, scalar
- QuantityResultDto
  - result, unit
- HistoryDto
  - id, operation, value/unit inputs, target/scalar, result, createdAt

### 8.2 Entities

- User
  - Id, Username, PasswordHash, PasswordSalt, Created, RefreshToken, RefreshTokenExpiry
- History
  - Id, Operation, Value1, Unit1, Value2, Unit2, TargetUnit, Scalar, Result, ResultUnit, CreatedAt

### 8.3 Internal History Model

- HistoryRecord (ModelLayer/Models)
  - Repository transfer model used between service and persistence layers

## 9) Quantity Domain Logic and Unit System

### 9.1 Core Types

- Unit (sealed)
  - Holds Name, Category, ConversionFactorToBase, OffsetToBase
  - Includes static unit definitions for supported categories
  - Methods:
    - ConvertToBaseUnit(value)
    - ConvertFromBaseUnit(baseValue)

- Quantity (abstract base)
  - Holds value and unit
  - Category-safe implementations:
    - ConvertTo
    - Add / Subtract
    - Divide(other quantity) -> scalar
    - Divide(scalar) -> same category quantity
  - Equality compares normalized base values with tolerance

- Category abstract types
  - Length, Weight, Volume, Temperature
  - Each implements factory-style CreateInstance for concrete types

### 9.2 Supported Units

Length:
- Feet, Inch, Yard, Meter, Centimeter

Weight:
- Kilogram, Gram, Pound

Volume:
- Litre, Millilitre, Gallon

Temperature:
- Celsius, Fahrenheit, Kelvin

### 9.3 Conversion Formula

Unit conversion uses factor and optional offset:

- To base: base = (value + offset) * factor
- From base: value = (base / factor) - offset

This supports both linear units and offset-aware temperature conversion.

## 10) Persistence, Caching, and Audit

### 10.1 DbContext

- QuantityDbContext exposes:
  - DbSet<History> Histories
  - DbSet<User> Users

### 10.2 HistoryRepository

Responsibilities:
- Save operation history rows
- Retrieve operation history ordered by CreatedAt descending
- Clear all history

Redis caching strategy:
- Cache key: history:all
- TTL: 30 minutes absolute
- Read path: cache-first, DB fallback, cache fill
- Write/clear path: DB update then cache invalidation

### 10.3 Audit Trigger Migration

Migration: 20260319115953_AddAuditTrigger

Creates:
- SystemAudit table
- Trigger trg_SystemAudit on Histories (INSERT/UPDATE/DELETE)
- JSON snapshots for old/new values in audit rows
- DENY UPDATE, DELETE on SystemAudit to public

## 11) Security Design

### 11.1 Authentication

- JWT Bearer authentication configured in Program.cs
- TokenValidationParameters validate:
  - issuer
  - audience
  - lifetime
  - signature key
- ClockSkew set to zero

### 11.2 Authorization

- [AllowAnonymous] on AuthController
- [Authorize] on QuantityController and HistoryController

### 11.3 Password Handling

- PasswordHasher uses HMACSHA256
- Salt generated using HMAC key
- Register stores hash+salt
- Login verifies computed hash with stored hash/salt

### 11.4 JWT Environment Variables

Expected at runtime:
- Jwt__Key
- Jwt__Issuer
- Jwt__Audience

DotNetEnv.Env.Load() is used at startup so values can be loaded from environment/.env.

## 12) Frontend Design and API Integration

Frontend lives under QuantityMeasurement.API/wwwroot.

Pages:
- index.html
  - Login and register forms
- home.html
  - Operation dashboard and history table

Scripts:
- assets/auth.js
  - Calls /api/auth/register and /api/auth/login
  - Stores qm_token in localStorage
  - Redirects to home.html on success
- assets/app.js
  - Loads dropdown options by category/operator
  - Calls quantity endpoints with bearer token
  - Calls history endpoints (GET, DELETE)
  - Renders result and history table
  - Handles token expiry by logout on 401

Styling:
- assets/styles.css

Notes:
- API base resolves from current origin when served by API host.
- Fallback is https://localhost:7051.
- CORS policy DevCors allows localhost/127.0.0.1 origins.

## 13) Setup and Run

Prerequisites:
- .NET SDK 8.x
- SQL Server instance
- Redis server

1. Restore and build:

```bash
dotnet restore
dotnet build QuantityMeasurementApp.sln
```

2. Configure environment values:
- Jwt__Key
- Jwt__Issuer
- Jwt__Audience

3. Ensure appsettings connection/caching values are correct:
- ConnectionStrings:DefaultConnection
- Redis:Configuration
- Redis:InstanceName

4. Run API:

```bash
dotnet run --project QuantityMeasurement.API
```

5. Open:
- Swagger: https://localhost:7051/swagger
- Frontend login page: https://localhost:7051/index.html
- Frontend home page: https://localhost:7051/home.html

## 14) Migrations and Database Commands

From the Infrastructure project directory:

Create migration:

```bash
dotnet ef migrations add <MigrationName> --startup-project ../QuantityMeasurement.API
```

Apply migrations:

```bash
dotnet ef database update --startup-project ../QuantityMeasurement.API
```

Reason startup project is API:
- API project contains runtime configuration used for DbContext creation.

## 15) Testing

Test project: QuantityMeasurement.Tests (NUnit)

Current suite is organized by UC files, including:
- UnitTestFeetUC-1.cs
- UnitTestInchesUC-2.cs
- UnitTestLengthEqualityUC-3.cs
- UnitTestCentimeterAndYardUC-4.cs
- UnitTestConverionUC-5.cs
- UnitTestUC-6.cs
- UnitTestUC-7.cs
- UnitTestUC-8.cs
- UnitTestUC-9.cs
- UnitTestVolumeUC-11.cs
- UnitTestSubDivUC-12.cs
- UnitTestTemperatureUC-14.cs

Run tests:

```bash
dotnet test QuantityMeasurement.Tests
```

## 16) Key Classes and Major Functions

API:
- Program
  - Startup composition, authentication, CORS, rate limiter, middleware pipeline
- AuthController
  - Register, Login endpoints
- QuantityController
  - Convert, Add, Subtract, DivideScalar, DivideQuantity endpoints
- HistoryController
  - Get, Delete history endpoints
- GlobalException
  - Centralized exception-to-ProblemDetails mapping
- TimeLogging
  - Request duration logging

Business:
- QuantityService
  - Convert, Add, Subtract, DivideByScalar, DivideByQuantity
  - ResolveUnit and CreateQuantity helpers
  - Persists history records for all operations
- AuthService
  - Register, Login, CreateToken
- PasswordHasher
  - HashPassword, VerifyPassword
- HistoryService
  - GetHistory, ClearHistory

Infrastructure:
- QuantityDbContext
  - EF Core sets for Users and Histories
- HistoryRepository
  - Save, GetHistory, ClearHistory with Redis cache integration

Model:
- Unit
  - Unit metadata + conversion formulas
- Quantity (QuantityAbstractBaseClass.cs)
  - Shared operation semantics and category safety
- Length/Weight/Volume/Temperature abstract subtypes
  - Category-specific type-safe operations and instance creation

## 17) Known Constraints and Suggested Improvements

Current implementation is functional, and the following are practical next improvements:

1. Auth request contracts
- Register/Login currently accept username/password through action parameters.
- Add dedicated DTO request models for cleaner API contracts.

2. Validation strategy
- Add DataAnnotations or FluentValidation for stronger pre-service input validation.

3. Auth exception semantics
- Consider throwing UnauthorizedAccessException for invalid login to align middleware mapping with 401.

4. DivideByQuantity micro-optimization
- QuantityService.DivideByQuantity calculates division twice; return the computed result variable directly.

5. History scope controls
- Add optional endpoint to clear specific history ranges or user-specific history if multi-user isolation is needed.

6. Token lifecycle
- User entity includes refresh token fields; add full refresh/revocation workflow if required.

---

This README reflects the current implemented state in the codebase as of March 28, 2026.
