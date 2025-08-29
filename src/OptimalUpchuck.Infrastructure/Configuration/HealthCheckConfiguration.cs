namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for health check monitoring.
/// </summary>
public class HealthCheckConfiguration
{
    /// <summary>
    /// Gets or sets the database health check settings.
    /// </summary>
    public HealthCheckSettings Database { get; set; } = new();

    /// <summary>
    /// Gets or sets the RabbitMQ health check settings.
    /// </summary>
    public HealthCheckSettings RabbitMQ { get; set; } = new();

    /// <summary>
    /// Gets or sets the Ollama health check settings.
    /// </summary>
    public HealthCheckSettings Ollama { get; set; } = new();
}

/// <summary>
/// Individual health check settings.
/// </summary>
public class HealthCheckSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether this health check is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeout in seconds for the health check.
    /// </summary>
    public int Timeout { get; set; } = 30;
}