#nullable enable

namespace OptimalUpchuck.Domain.Events;

/// <summary>
/// Domain event raised when an elevation proposal is approved
/// </summary>
public record ProposalApprovedEvent(
    Guid ProposalId,
    string AgentType,
    string OutputDestination,
    string? ReviewerComments,
    DateTime ApprovedAt) : IDomainEvent;