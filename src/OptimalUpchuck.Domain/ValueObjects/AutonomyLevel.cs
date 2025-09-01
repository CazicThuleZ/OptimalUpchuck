#nullable enable

namespace OptimalUpchuck.Domain.ValueObjects;

/// <summary>
/// Represents the autonomy level of an agent configuration
/// </summary>
public enum AutonomyLevel
{
    /// <summary>
    /// All agent actions require human review before execution
    /// </summary>
    ReviewRequired = 0,

    /// <summary>
    /// Agent can execute some actions automatically based on confidence thresholds
    /// </summary>
    SemiAutonomous = 1,

    /// <summary>
    /// Agent can execute all actions automatically if confidence exceeds threshold
    /// </summary>
    FullyAutonomous = 2
}