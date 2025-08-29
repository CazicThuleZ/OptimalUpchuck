using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OptimalUpchuck.Ui.Controllers;

/// <summary>
/// Controller for health check endpoints and system status monitoring.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    /// <param name="healthCheckService">The health check service.</param>
    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Gets the overall system health status.
    /// </summary>
    /// <returns>Health status response.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();
        
        var response = new
        {
            Status = healthReport.Status.ToString(),
            Duration = healthReport.TotalDuration.TotalMilliseconds,
            Checks = healthReport.Entries.Select(entry => new
            {
                Name = entry.Key,
                Status = entry.Value.Status.ToString(),
                Duration = entry.Value.Duration.TotalMilliseconds,
                Description = entry.Value.Description,
                Data = entry.Value.Data
            })
        };

        return healthReport.Status == HealthStatus.Healthy 
            ? Ok(response) 
            : StatusCode(503, response);
    }

    /// <summary>
    /// Gets a simple health check response for load balancers.
    /// </summary>
    /// <returns>Simple health status.</returns>
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Gets the health status of a specific service.
    /// </summary>
    /// <param name="service">The service name to check.</param>
    /// <returns>Service-specific health status.</returns>
    [HttpGet("{service}")]
    public async Task<IActionResult> GetServiceHealth(string service)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync(check => 
            check.Name.Equals(service, StringComparison.OrdinalIgnoreCase));

        if (!healthReport.Entries.Any())
        {
            return NotFound(new { Message = $"Health check '{service}' not found" });
        }

        var entry = healthReport.Entries.First();
        var response = new
        {
            Name = entry.Key,
            Status = entry.Value.Status.ToString(),
            Duration = entry.Value.Duration.TotalMilliseconds,
            Description = entry.Value.Description,
            Data = entry.Value.Data
        };

        return entry.Value.Status == HealthStatus.Healthy 
            ? Ok(response) 
            : StatusCode(503, response);
    }
}