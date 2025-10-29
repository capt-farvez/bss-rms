using BssRms.Application.Interfaces;
using BssRms.Application.Services;
using BssRms.Domain.Interfaces;
using BssRms.Infrastructure.Data;
using BssRms.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var currentDirEnv = Path.Combine(Directory.GetCurrentDirectory(), ".env");
var solutionRootEnv = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", ".env");

if (File.Exists(currentDirEnv))
{
    DotNetEnv.Env.Load(currentDirEnv);
}
else if (File.Exists(solutionRootEnv))
{
    DotNetEnv.Env.Load(solutionRootEnv);
}

var builder = WebApplication.CreateBuilder(args);

// Configure Database
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DB_CONNECTION_STRING is not set. Please check your .env file.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Repositories
builder.Services.AddScoped<ITestTableRepository, TestTableRepository>();

// Register Services
builder.Services.AddScoped<ITestTableService, TestTableService>();

// Add Controllers
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Controllers
app.MapControllers();

app.Run();
