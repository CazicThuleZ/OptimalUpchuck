#nullable enable

namespace OptimalUpchuck.Domain.ValueObjects;

/// <summary>
/// Represents the review status of an elevation proposal
/// </summary>
public enum ReviewStatus
{
    /// <summary>Proposal is awaiting human review</summary>
    Pending = 0,
    /// <summary>Proposal has been approved for publication</summary>
    Approved = 1,
    /// <summary>Proposal has been denied and will not be published</summary>
    Denied = 2,
    /// <summary>Proposal has expired without review</summary>
    Expired = 3
}