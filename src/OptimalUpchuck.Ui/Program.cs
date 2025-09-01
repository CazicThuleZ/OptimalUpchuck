
using OptimalUpchuck.Infrastructure.Configuration;
using OptimalUpchuck.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
        npgsqlOptions.MigrationsAssembly("OptimalUpchuck.Infrastructure");
    });
    
    // Enable sensitive data logging only in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure strongly-typed configuration
builder.Services.Configure<RabbitMQConfiguration>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<SemanticKernelConfiguration>(
    builder.Configuration.GetSection("SemanticKernel"));
builder.Services.Configure<AgentConfiguration>(
    builder.Configuration.GetSection("AgentConfiguration"));
builder.Services.Configure<ObsidianVaultConfiguration>(
    builder.Configuration.GetSection("ObsidianVault"));
builder.Services.Configure<HealthCheckConfiguration>(
    builder.Configuration.GetSection("HealthChecks"));

// Add configuration validation
builder.Services.AddSingleton<IValidateOptions<RabbitMQConfiguration>, ConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<SemanticKernelConfiguration>, ConfigurationValidator>();
builder.Services.AddSingleton<IValidateOptions<ObsidianVaultConfiguration>, ConfigurationValidator>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "database",
        tags: new[] { "db", "postgresql" })
    .AddUrlGroup(
        uri: new Uri($"{builder.Configuration.GetSection("SemanticKernel")["OllamaApiUrl"]}/api/tags"),
        name: "ollama",
        tags: new[] { "ai", "ollama" });

// Add health checks UI
builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/Error404");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// Configure health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Configure health checks UI
app.MapHealthChecksUI(config =>
{
    config.UIPath = "/health-ui";
    config.ApiPath = "/health-ui-api";
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboards}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
