#nullable enable

using FluentAssertions;
using OptimalUpchuck.Domain.ValueObjects;
using Xunit;

namespace OptimalUpchuck.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for ConfidenceScore value object
/// </summary>
public class ConfidenceScoreTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(0.25)]
    [InlineData(0.75)]
    public void Constructor_WithValidValue_SetsValueCorrectly(decimal value)
    {
        // Act
        var score = new ConfidenceScore(value);

        // Assert
        score.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(-1.0)]
    [InlineData(2.0)]
    public void Constructor_WithInvalidValue_ThrowsArgumentOutOfRangeException(decimal value)
    {
        // Act & Assert
        var act = () => new ConfidenceScore(value);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Confidence score must be between 0.0 and 1.0*");
    }

    [Fact]
    public void Constructor_RoundsToTwoDecimalPlaces()
    {
        // Arrange
        var value = 0.12345m;

        // Act
        var score = new ConfidenceScore(value);

        // Assert
        score.Value.Should().Be(0.12m);
    }

    [Fact]
    public void ImplicitOperator_ConvertsToDecimal()
    {
        // Arrange
        var score = new ConfidenceScore(0.85m);

        // Act
        decimal value = score;

        // Assert
        value.Should().Be(0.85m);
    }

    [Fact]
    public void ExplicitOperator_ConvertsFromDecimal()
    {
        // Arrange
        const decimal value = 0.75m;

        // Act
        var score = (ConfidenceScore)value;

        // Assert
        score.Value.Should().Be(0.75m);
    }

    [Theory]
    [InlineData(0.8, true)]
    [InlineData(0.9, true)]
    [InlineData(1.0, true)]
    [InlineData(0.79, false)]
    [InlineData(0.5, false)]
    [InlineData(0.0, false)]
    public void IsHighConfidence_ReturnsCorrectValue(decimal value, bool expected)
    {
        // Arrange
        var score = new ConfidenceScore(value);

        // Act & Assert
        score.IsHighConfidence.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.0, true)]
    [InlineData(0.25, true)]
    [InlineData(0.49, true)]
    [InlineData(0.5, false)]
    [InlineData(0.75, false)]
    [InlineData(1.0, false)]
    public void IsLowConfidence_ReturnsCorrectValue(decimal value, bool expected)
    {
        // Arrange
        var score = new ConfidenceScore(value);

        // Act & Assert
        score.IsLowConfidence.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.0, "0%")]
    [InlineData(0.5, "50%")]
    [InlineData(1.0, "100%")]
    [InlineData(0.85, "85%")]
    public void ToString_ReturnsPercentageString(decimal value, string expected)
    {
        // Arrange
        var score = new ConfidenceScore(value);

        // Act
        var result = score.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Equals_WithSameValue_ReturnsTrue()
    {
        // Arrange
        var score1 = new ConfidenceScore(0.75m);
        var score2 = new ConfidenceScore(0.75m);

        // Act & Assert
        score1.Should().Be(score2);
        score1.Equals(score2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ReturnsFalse()
    {
        // Arrange
        var score1 = new ConfidenceScore(0.75m);
        var score2 = new ConfidenceScore(0.85m);

        // Act & Assert
        score1.Should().NotBe(score2);
        score1.Equals(score2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValue_ReturnsSameHashCode()
    {
        // Arrange
        var score1 = new ConfidenceScore(0.75m);
        var score2 = new ConfidenceScore(0.75m);

        // Act & Assert
        score1.GetHashCode().Should().Be(score2.GetHashCode());
    }
}