## Setup Instructions (Local Development)

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