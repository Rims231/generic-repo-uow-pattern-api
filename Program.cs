using generic_repo_uow_pattern_api.CustomHealthCheck;
using generic_repo_uow_pattern_api.Data;
using generic_repo_uow_pattern_api.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHttpClient<ApiHealthCheck>();
builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("API Health Check", tags: new[] { "self" })
    .AddDbContextCheck<MyDbContext>("Database Check", tags: new[] { "db" });

builder.Services.AddHealthChecksUI(options =>
{
    options.AddHealthCheckEndpoint("API Health Check", "https://localhost:7024/health");
    options.AddHealthCheckEndpoint("Full Health Check", "https://localhost:7024/health/full");
})
.AddInMemoryStorage();

// Repositories & UoW
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Database
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DB only — safe for internal/infra monitoring (no self-referencing loop)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

// All checks — intended for external monitoring tools only
app.MapHealthChecks("/health/full", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-ui-api";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();