namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Microsoft Semantic Kernel and AI model integration.
/// </summary>
public class SemanticKernelConfiguration
{
    /// <summary>
    /// Gets or sets the Ollama API URL.
    /// </summary>
    public string OllamaApiUrl { get; set; } = "http://localhost:11434";

    /// <summary>
    /// Gets or sets the default AI model to use.
    /// </summary>
    public string DefaultModel { get; set; } = "llama3.2";

    /// <summary>
    /// Gets or sets the request timeout in milliseconds.
    /// </summary>
    public int RequestTimeout { get; set; } = 120000;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}