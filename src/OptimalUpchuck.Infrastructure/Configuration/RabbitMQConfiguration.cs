namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for RabbitMQ message broker.
/// </summary>
public class RabbitMQConfiguration
{
    /// <summary>
    /// Gets or sets the RabbitMQ host name.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the RabbitMQ port number.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Gets or sets the RabbitMQ user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the RabbitMQ password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the RabbitMQ virtual host.
    /// </summary>
    public string VirtualHost { get; set; } = "/";

    /// <summary>
    /// Gets or sets the connection timeout in milliseconds.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30000;

    /// <summary>
    /// Gets or sets the requested heartbeat interval in seconds.
    /// </summary>
    public ushort RequestedHeartbeat { get; set; } = 60;
}