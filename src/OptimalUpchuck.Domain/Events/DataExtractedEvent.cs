#nullable enable

namespace OptimalUpchuck.Domain.Events;

/// <summary>
/// Domain event raised when data is extracted from a source file
/// </summary>
public record DataExtractedEvent(
    Guid DataId,
    string AgentType,
    string SourceFilePath,
    string DataType,
    string DataValue,
    decimal ConfidenceScore,
    DateTime ExtractedAt) : IDomainEvent;