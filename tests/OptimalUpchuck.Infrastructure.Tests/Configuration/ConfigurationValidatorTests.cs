using FluentAssertions;
using Microsoft.Extensions.Options;
using OptimalUpchuck.Infrastructure.Configuration;

namespace OptimalUpchuck.Infrastructure.Tests.Configuration;

/// <summary>
/// Tests for the ConfigurationValidator class.
/// </summary>
public class ConfigurationValidatorTests
{
    private readonly ConfigurationValidator _validator = new();

    [Fact]
    public void Validate_RabbitMQConfiguration_WithValidSettings_ShouldReturnSuccess()
    {
        // Arrange
        var config = new RabbitMQConfiguration
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "user",
            Password = "password",
            VirtualHost = "/",
            ConnectionTimeout = 30000,
            RequestedHeartbeat = 60
        };

        // Act
        var result = _validator.Validate(nameof(RabbitMQConfiguration), config);

        // Assert
        result.Should().Be(ValidateOptionsResult.Success);
    }

    [Fact]
    public void Validate_RabbitMQConfiguration_WithEmptyHostName_ShouldReturnFail()
    {
        // Arrange
        var config = new RabbitMQConfiguration
        {
            HostName = "",
            Port = 5672,
            UserName = "user",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(nameof(RabbitMQConfiguration), config);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("RabbitMQ HostName is required");
    }

    [Fact]
    public void Validate_RabbitMQConfiguration_WithInvalidPort_ShouldReturnFail()
    {
        // Arrange
        var config = new RabbitMQConfiguration
        {
            HostName = "localhost",
            Port = 0,
            UserName = "user",
            Password = "password"
        };

        // Act
        var result = _validator.Validate(nameof(RabbitMQConfiguration), config);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("RabbitMQ Port must be between 1 and 65535");
    }

    [Fact]
    public void Validate_SemanticKernelConfiguration_WithValidSettings_ShouldReturnSuccess()
    {
        // Arrange
        var config = new SemanticKernelConfiguration
        {
            OllamaApiUrl = "http://localhost:11434",
            DefaultModel = "llama3.2",
            RequestTimeout = 120000,
            MaxRetries = 3
        };

        // Act
        var result = _validator.Validate(nameof(SemanticKernelConfiguration), config);

        // Assert
        result.Should().Be(ValidateOptionsResult.Success);
    }

    [Fact]
    public void Validate_SemanticKernelConfiguration_WithInvalidUrl_ShouldReturnFail()
    {
        // Arrange
        var config = new SemanticKernelConfiguration
        {
            OllamaApiUrl = "invalid-url",
            DefaultModel = "llama3.2",
            RequestTimeout = 120000,
            MaxRetries = 3
        };

        // Act
        var result = _validator.Validate(nameof(SemanticKernelConfiguration), config);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("SemanticKernel OllamaApiUrl must be a valid URL");
    }

    [Fact]
    public void Validate_ObsidianVaultConfiguration_WithValidSettings_ShouldReturnSuccess()
    {
        // Arrange
        var config = new ObsidianVaultConfiguration
        {
            RawVaultPath = "/path/to/raw",
            PristineVaultPath = "/path/to/pristine",
            WatchEnabled = true,
            FileExtensions = new[] { ".md" },
            ExcludePatterns = new[] { ".*", "_*" }
        };

        // Act
        var result = _validator.Validate(nameof(ObsidianVaultConfiguration), config);

        // Assert
        result.Should().Be(ValidateOptionsResult.Success);
    }

    [Fact]
    public void Validate_ObsidianVaultConfiguration_WithEmptyPaths_ShouldReturnFail()
    {
        // Arrange
        var config = new ObsidianVaultConfiguration
        {
            RawVaultPath = "",
            PristineVaultPath = "",
            FileExtensions = new[] { ".md" },
            ExcludePatterns = new[] { ".*" }
        };

        // Act
        var result = _validator.Validate(nameof(ObsidianVaultConfiguration), config);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain("ObsidianVault RawVaultPath is required");
        result.Failures.Should().Contain("ObsidianVault PristineVaultPath is required");
    }
}