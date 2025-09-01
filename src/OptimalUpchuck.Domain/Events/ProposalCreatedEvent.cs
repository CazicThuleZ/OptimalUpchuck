#nullable enable

namespace OptimalUpchuck.Domain.Events;

/// <summary>
/// Domain event raised when an elevation proposal is created
/// </summary>
public record ProposalCreatedEvent(
    Guid ProposalId,
    string AgentType,
    string SourceFilePath,
    decimal ConfidenceScore,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
}