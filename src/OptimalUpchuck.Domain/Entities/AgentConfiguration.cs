#nullable enable

using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Domain.Entities;

/// <summary>
/// Represents configuration settings for an agent
/// </summary>
public class AgentConfiguration
{
    public Guid Id { get; private set; }
    public string AgentType { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public AutonomyLevel AutonomyLevel { get; private set; }
    public ConfidenceScore ConfidenceThreshold { get; private set; }
    public string ConfigurationJson { get; private set; } = string.Empty;
    public string? ModelParameters { get; private set; }
    public string? ProcessingRules { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public int Version { get; private set; }

    // Navigation properties
    public ICollection<ElevationProposal> ElevationProposals { get; private set; } = new List<ElevationProposal>();
    public ICollection<ExtractedData> ExtractedData { get; private set; } = new List<ExtractedData>();

    // Private constructor for EF Core
    private AgentConfiguration() { }

    public AgentConfiguration(
        string agentType,
        AutonomyLevel autonomyLevel,
        ConfidenceScore confidenceThreshold,
        string configurationJson,
        string? modelParameters = null,
        string? processingRules = null,
        bool isEnabled = true)
    {
        if (string.IsNullOrWhiteSpace(agentType))
            throw new ArgumentException("Agent type cannot be empty", nameof(agentType));
        if (string.IsNullOrWhiteSpace(configurationJson))
            throw new ArgumentException("Configuration JSON cannot be empty", nameof(configurationJson));

        Id = Guid.NewGuid();
        AgentType = agentType;
        IsEnabled = isEnabled;
        AutonomyLevel = autonomyLevel;
        ConfidenceThreshold = confidenceThreshold;
        ConfigurationJson = configurationJson;
        ModelParameters = modelParameters;
        ProcessingRules = processingRules;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Version = 1;
    }

    public void UpdateConfiguration(
        AutonomyLevel? autonomyLevel = null,
        ConfidenceScore? confidenceThreshold = null,
        string? configurationJson = null,
        string? modelParameters = null,
        string? processingRules = null,
        bool? isEnabled = null)
    {
        if (autonomyLevel.HasValue)
            AutonomyLevel = autonomyLevel.Value;
        
        if (confidenceThreshold.HasValue)
            ConfidenceThreshold = confidenceThreshold.Value;
        
        if (!string.IsNullOrWhiteSpace(configurationJson))
            ConfigurationJson = configurationJson;
        
        if (modelParameters is not null)
            ModelParameters = modelParameters;
        
        if (processingRules is not null)
            ProcessingRules = processingRules;
        
        if (isEnabled.HasValue)
            IsEnabled = isEnabled.Value;

        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    public void Enable() => UpdateConfiguration(isEnabled: true);
    public void Disable() => UpdateConfiguration(isEnabled: false);
}