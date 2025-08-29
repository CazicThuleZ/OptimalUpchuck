namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for agent behavior and parameters.
/// </summary>
public class AgentConfiguration
{
    /// <summary>
    /// Gets or sets the Statistics Agent configuration.
    /// </summary>
    public AgentSettings StatisticsAgent { get; set; } = new();

    /// <summary>
    /// Gets or sets the Blogging Agent configuration.
    /// </summary>
    public AgentSettings BloggingAgent { get; set; } = new();
}

/// <summary>
/// Individual agent settings and parameters.
/// </summary>
public class AgentSettings
{
    /// <summary>
    /// Gets or sets the autonomy level for the agent.
    /// </summary>
    public string AutonomyLevel { get; set; } = "ReviewRequired";

    /// <summary>
    /// Gets or sets the confidence threshold for autonomous action.
    /// </summary>
    public double ConfidenceThreshold { get; set; } = 0.75;

    /// <summary>
    /// Gets or sets the AI model to use for this agent.
    /// </summary>
    public string Model { get; set; } = "llama3.2";

    /// <summary>
    /// Gets or sets the model temperature (randomness) setting.
    /// </summary>
    public double Temperature { get; set; } = 0.3;

    /// <summary>
    /// Gets or sets the maximum number of tokens to generate.
    /// </summary>
    public int MaxTokens { get; set; } = 1000;
}