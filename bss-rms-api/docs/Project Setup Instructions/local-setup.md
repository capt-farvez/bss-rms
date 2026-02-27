## Setup Instructions (Local Development)

### Project Directory Structure

```
bss-rms/
└── bss-rms-api/                        # API root (contains .sln)
    ├── BssRmsApi.sln
    ├── .env.example
    ├── docs/
    └── src/
        ├── BssRms.Api/BssRms.Api/      # Startup project (.csproj, Program.cs, Controllers)
        ├── BssRms.Application/BssRms.Application/
        ├── BssRms.Domain/BssRms.Domain/
        └── BssRms.Infrastructure/BssRms.Infrastructure/  # DbContext, Migrations
```

### 1. Clone the Repository

```bash
git clone https://github.com/capt-farvez/bss-rms
cd bss-rms
```

### 2. Set Up Environment Variables

From the `bss-rms-api/` directory, copy the `.env.example` file to `.env`:

```bash
cd bss-rms-api
cp .env.example .env
```

Edit the `.env` file and add your database connection string:

```
DB_CONNECTION_STRING="Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD"
```

### 3. Restore Dependencies

Run from the `bss-rms-api/` directory (where `BssRmsApi.sln` is located):

```bash
# bss-rms/bss-rms-api/
dotnet restore
```

### 4. Run the Application

#### Option A: Using .NET CLI

From the `bss-rms-api/` directory, run:

```bash
# bss-rms/bss-rms-api/
# Default (http profile) — runs on port 5027
dotnet run --project src/BssRms.Api/BssRms.Api

# Or with HTTPS profile — runs on port 7212
dotnet run --project src/BssRms.Api/BssRms.Api --launch-profile https
```

The API will be available at:
- **HTTP (default)**: `http://localhost:5027`
- **HTTPS**: `https://localhost:7212` (requires `--launch-profile https`)
- **Swagger UI**: `http://localhost:5027/swagger` or `https://localhost:7212/swagger`

#### Option B: Using Visual Studio

1. Open `bss-rms-api/BssRmsApi.sln` in Visual Studio
2. Ensure **BssRms.Api** is set as the startup project (right-click -> Set as Startup Project)
3. Select the **https** launch profile from the dropdown (next to the Start button)
4. Press `F5` or click the **Start** button
5. Your browser will automatically open to the Swagger UI

#### Option C: Using Visual Studio Code (Recommended)

1. Open the `bss-rms-api` folder in VS Code
2. Install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension
3. Press `F5` to start debugging, or use the terminal:
   ```bash
   # bss-rms/bss-rms-api/
   dotnet run --project src/BssRms.Api/BssRms.Api
   ```
4. Open your browser to `http://localhost:5027/swagger`

## API Documentation

Once the application is running, you can access:

- **Swagger UI (HTTP)**: `http://localhost:5027/swagger` - Interactive API documentation
- **Swagger UI (HTTPS)**: `https://localhost:7212/swagger` (if using `https` launch profile)
- **HTTP**: `http://localhost:5027`
- **HTTPS**: `https://localhost:7212`

### Using Swagger UI with Authentication

Most API endpoints require authentication. Follow these steps to use Swagger:

1. Open Swagger UI at `http://localhost:5027/swagger`
2. Find the **Auth > /api/Auth/login** endpoint and execute it with valid credentials:
   ```json
   {
     "email": "admin@mail.com",
     "password": "your_password"
   }
   ```
3. Copy the `token` value from the response
4. Click the **Authorize** button (lock icon) at the top of the Swagger page
5. In the value field, enter:
   ```
   Bearer <your_token>
   ```
   For example: `Bearer eyJhbGciOiJIUzI1NiIs...`
6. Click **Authorize**, then **Close**
7. Now all protected endpoints will work with your token

> **Note:** Tokens expire after 60 minutes. If you get a `401 Unauthorized` response, repeat the login steps to get a new token.


## Migrations

All migration commands must be run from the **Infrastructure project directory**:

```bash
# Navigate from the API root to the Infrastructure project
cd bss-rms-api/src/BssRms.Infrastructure/BssRms.Infrastructure
```

### Create a migration:
```bash
# bss-rms/bss-rms-api/src/BssRms.Infrastructure/BssRms.Infrastructure/
dotnet ef migrations add MigrationName --startup-project ../../BssRms.Api/BssRms.Api
```

### Apply migrations to the database:
```bash
# bss-rms/bss-rms-api/src/BssRms.Infrastructure/BssRms.Infrastructure/
dotnet ef database update --startup-project ../../BssRms.Api/BssRms.Api
```

### List migrations:
```bash
# bss-rms/bss-rms-api/src/BssRms.Infrastructure/BssRms.Infrastructure/
dotnet ef migrations list --startup-project ../../BssRms.Api/BssRms.Api
```

### Remove last migration:
```bash
# bss-rms/bss-rms-api/src/BssRms.Infrastructure/BssRms.Infrastructure/
dotnet ef migrations remove --startup-project ../../BssRms.Api/BssRms.Api
```