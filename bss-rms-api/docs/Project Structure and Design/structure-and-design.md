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