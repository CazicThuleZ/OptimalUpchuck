#nullable enable

namespace OptimalUpchuck.Domain.ValueObjects;

/// <summary>
/// Represents a confidence score value object with range validation (0.0 to 1.0)
/// </summary>
public readonly record struct ConfidenceScore
{
    /// <summary>
    /// The decimal value of the confidence score (0.0 to 1.0)
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Creates a new confidence score with validation
    /// </summary>
    /// <param name="value">The confidence value between 0.0 and 1.0</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is outside 0.0-1.0 range</exception>
    public ConfidenceScore(decimal value)
    {
        if (value < 0.0m || value > 1.0m)
        {
            throw new ArgumentOutOfRangeException(nameof(value), 
                "Confidence score must be between 0.0 and 1.0");
        }

        Value = Math.Round(value, 2);
    }

    /// <summary>
    /// Implicitly converts confidence score to decimal
    /// </summary>
    /// <param name="score">The confidence score</param>
    public static implicit operator decimal(ConfidenceScore score) => score.Value;
    /// <summary>
    /// Explicitly converts decimal to confidence score
    /// </summary>
    /// <param name="value">The decimal value</param>
    public static explicit operator ConfidenceScore(decimal value) => new(value);

    /// <summary>
    /// Gets whether this is a high confidence score (>= 0.8)
    /// </summary>
    public bool IsHighConfidence => Value >= 0.8m;
    /// <summary>
    /// Gets whether this is a low confidence score (< 0.5)
    /// </summary>
    public bool IsLowConfidence => Value < 0.5m;

    /// <summary>
    /// Returns the confidence score as a percentage string
    /// </summary>
    /// <returns>Percentage representation of the confidence score</returns>
    public override string ToString() => $"{Value:P0}";
}