namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Configuration settings for Obsidian vault integration.
/// </summary>
public class ObsidianVaultConfiguration
{
    /// <summary>
    /// Gets or sets the path to the raw Obsidian vault (input).
    /// </summary>
    public string RawVaultPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the pristine Obsidian vault (output).
    /// </summary>
    public string PristineVaultPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether file watching is enabled.
    /// </summary>
    public bool WatchEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the file extensions to monitor.
    /// </summary>
    public string[] FileExtensions { get; set; } = [".md"];

    /// <summary>
    /// Gets or sets the file patterns to exclude from monitoring.
    /// </summary>
    public string[] ExcludePatterns { get; set; } = [".*", "_*", "temp*"];
}