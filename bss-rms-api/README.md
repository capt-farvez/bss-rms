# BSS RMS API

Restaurant Management System API built with ASP.NET Core 9.0

## Project Structure

```
bss-rms-api/
├── src/
│   ├── BssRms.Api/              # Web API layer (Controllers, Middleware)
│   ├── BssRms.Application/      # Business logic, DTOs, Services, AutoMapper
│   ├── BssRms.Domain/           # Domain entities, Repository interfaces
│   └── BssRms.Infrastructure/   # EF Core, DbContext, Repository implementations, Migrations
├── .env                         # Environment variables (not in git)
├── .env.example                 # Environment variables template
└── BssRmsApi.sln                # Solution file
```

### Architecture Layers

#### **BssRms.Api** (Presentation Layer)
- Controllers (controllers, HTTP request/response handling)
- Middleware and filters
- API documentation (Swagger/OpenAPI)

#### **BssRms.Application** (Application/Business Layer)
- DTOs (Data Transfer Objects)
- AutoMapper profiles for object mapping
- Service interfaces and implementations
- Business logic and validation rules

#### **BssRms.Domain** (Domain Layer)
- Entity models
- Repository interfaces (IEmployeeRepository, IFoodRepository, etc.)
- Domain logic and business rules

#### **BssRms.Infrastructure** (Infrastructure/Data Layer)
- ApplicationDbContext (EF Core)
- Repository pattern implementations
- Database configurations
- Entity Framework migrations
- Data access logic

### Architecture Flow Diagram

``` 
                  HTTP Request
                       ↓
    ┌────────────────────────────────────────┐
    │         BssRms.Api (API Layer)         │
    │  ┌─────────────────────────────────┐   │
    │  │         Controller              │   │
    │  └─────────────────────────────────┘   │
    └──────────────────┬─────────────────────┘
                       ↓ calls
    ┌────────────────────────────────────────┐
    │   BssRms.Application (Business Layer)  │
    │  ┌─────────────────────────────────┐   │
    │  │       IService (Interface)      │   │
    │  │              ↓                  │   │
    │  │     Service Implementation      │   │
    │  └─────────────────────────────────┘   │
    └──────────────────┬─────────────────────┘
                       ↓ calls
    ┌────────────────────────────────────────┐
    │     BssRms.Domain (Domain Layer)       │
    │  ┌─────────────────────────────────┐   │
    │  │    IRepository (Interface)      │   │
    │  └─────────────────────────────────┘   │
    └──────────────────┬─────────────────────┘
                       ↓ implemented by
    ┌────────────────────────────────────────┐
    │  BssRms.Infrastructure (Data Layer)    │
    │  ┌─────────────────────────────────┐   │
    │  │   Repository Implementation     │   │
    │  └──────────────┬──────────────────┘   │
    └─────────────────┼──────────────────────┘
                      ↓ queries
    ┌────────────────────────────────────────┐
    │            Database (SQL Server)       │
    └─────────────────┬──────────────────────┘
                      ↓
                 Returns Data
                      ↓
                JSON Response
```

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server database
- Git

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/capt-farvez/bss-rms
cd bss-rms/bss-rms-api
```

### 2. Set Up Environment Variables

Copy the `.env.example` file to `.env`:

```bash
cp .env.example .env
```

Edit the `.env` file and add your database connection string:

```
DB_CONNECTION_STRING="Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD"
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

#### Option A: Using .NET CLI

```bash
cd src/BssRms.Api/BssRms.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7212`
- Swagger UI: `https://localhost:7212/swagger`

#### Option B: Using Visual Studio

1. Open `BssRmsApi.sln` in Visual Studio
2. Ensure **BssRms.Api** is set as the startup project (right-click -> Set as Startup Project)
3. Press `F5` or click the **Start** button
4. Your browser will automatically open to the Swagger UI

#### Option C: Using Visual Studio Code

1. Open the `bss-rms-api` folder in VS Code
2. Install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension
3. Press `F5` to start debugging, or use the terminal:
   ```bash
   cd src/BssRms.Api/BssRms.Api
   dotnet run
   ```
4. Open your browser to `https://localhost:7212/swagger`

## API Documentation

Once the application is running, you can access:

- **Swagger UI**: `https://localhost:7212/swagger` - Interactive API documentation
- **HTTPS**: `https://localhost:7212`


## Migrations
To add a new migration, navigate to the `BssRms.Infrastructure` project directory and run:

### Create a migration with the following command:
```bash
dotnet ef migrations add MigrationName --startup-project ../../BssRms.Api/BssRms.Api
```
### Apply migrations to the database with:
```bash
dotnet ef database update --startup-project ../../BssRms.Api/BssRms.Api
```
### List Migrations:
```bash
dotnet ef migrations list --startup-project ../../BssRms.Api/BssRms.Api
```
### Remove last Migration:
```bash
dotnet ef migrations remove --startup-project ../../BssRms.Api/BssRms.Api
```