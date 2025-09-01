#nullable enable

using OptimalUpchuck.Domain.Events;
using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Domain.Entities;

/// <summary>
/// Represents structured data extracted from source files by agents
/// </summary>
public class ExtractedData
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public string SourceFilePath { get; private set; } = string.Empty;
    public string AgentType { get; private set; } = string.Empty;
    public string DataType { get; private set; } = string.Empty;
    public string DataValue { get; private set; } = string.Empty;
    public string DataUom { get; private set; } = string.Empty;
    public ConfidenceScore ConfidenceScore { get; private set; }
    public DateTime ExtractedAt { get; private set; }
    public string? Context { get; private set; }
    public Guid AgentConfigurationId { get; private set; }
    public string? ProcessingMetadata { get; private set; }

    // Navigation property
    public AgentConfiguration? AgentConfiguration { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Private constructor for EF Core
    private ExtractedData() { }

    public ExtractedData(
        string sourceFilePath,
        string agentType,
        string dataType,
        string dataValue,
        string dataUom,
        ConfidenceScore confidenceScore,
        Guid agentConfigurationId,
        string? context = null,
        string? processingMetadata = null)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
            throw new ArgumentException("Source file path cannot be empty", nameof(sourceFilePath));
        if (string.IsNullOrWhiteSpace(agentType))
            throw new ArgumentException("Agent type cannot be empty", nameof(agentType));
        if (string.IsNullOrWhiteSpace(dataType))
            throw new ArgumentException("Data type cannot be empty", nameof(dataType));
        if (string.IsNullOrWhiteSpace(dataValue))
            throw new ArgumentException("Data value cannot be empty", nameof(dataValue));
        if (string.IsNullOrWhiteSpace(dataUom))
            throw new ArgumentException("Data unit of measure cannot be empty", nameof(dataUom));
        if (agentConfigurationId == Guid.Empty)
            throw new ArgumentException("Agent configuration ID cannot be empty", nameof(agentConfigurationId));

        Id = Guid.NewGuid();
        SourceFilePath = sourceFilePath;
        AgentType = agentType;
        DataType = dataType;
        DataValue = dataValue;
        DataUom = dataUom;
        ConfidenceScore = confidenceScore;
        ExtractedAt = DateTime.UtcNow;
        Context = context;
        AgentConfigurationId = agentConfigurationId;
        ProcessingMetadata = processingMetadata;

        _domainEvents.Add(new DataExtractedEvent(Id, AgentType, SourceFilePath, DataType, DataValue, ConfidenceScore, ExtractedAt));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}