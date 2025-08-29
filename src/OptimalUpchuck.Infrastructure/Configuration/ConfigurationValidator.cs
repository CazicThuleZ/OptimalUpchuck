using Microsoft.Extensions.Options;

namespace OptimalUpchuck.Infrastructure.Configuration;

/// <summary>
/// Validates configuration settings for required values and proper formats.
/// </summary>
public class ConfigurationValidator : 
    IValidateOptions<RabbitMQConfiguration>,
    IValidateOptions<SemanticKernelConfiguration>,
    IValidateOptions<ObsidianVaultConfiguration>
{
    /// <summary>
    /// Validates RabbitMQ configuration settings.
    /// </summary>
    /// <param name="name">The configuration section name.</param>
    /// <param name="options">The configuration options to validate.</param>
    /// <returns>Validation result.</returns>
    public ValidateOptionsResult Validate(string? name, RabbitMQConfiguration options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.HostName))
            errors.Add("RabbitMQ HostName is required");

        if (options.Port <= 0 || options.Port > 65535)
            errors.Add("RabbitMQ Port must be between 1 and 65535");

        if (string.IsNullOrWhiteSpace(options.UserName))
            errors.Add("RabbitMQ UserName is required");

        if (string.IsNullOrWhiteSpace(options.Password))
            errors.Add("RabbitMQ Password is required");

        if (options.ConnectionTimeout <= 0)
            errors.Add("RabbitMQ ConnectionTimeout must be greater than 0");

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }

    /// <summary>
    /// Validates Semantic Kernel configuration settings.
    /// </summary>
    /// <param name="name">The configuration section name.</param>
    /// <param name="options">The configuration options to validate.</param>
    /// <returns>Validation result.</returns>
    public ValidateOptionsResult Validate(string? name, SemanticKernelConfiguration options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.OllamaApiUrl))
            errors.Add("SemanticKernel OllamaApiUrl is required");

        if (!Uri.TryCreate(options.OllamaApiUrl, UriKind.Absolute, out _))
            errors.Add("SemanticKernel OllamaApiUrl must be a valid URL");

        if (string.IsNullOrWhiteSpace(options.DefaultModel))
            errors.Add("SemanticKernel DefaultModel is required");

        if (options.RequestTimeout <= 0)
            errors.Add("SemanticKernel RequestTimeout must be greater than 0");

        if (options.MaxRetries < 0)
            errors.Add("SemanticKernel MaxRetries must be greater than or equal to 0");

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }

    /// <summary>
    /// Validates Obsidian Vault configuration settings.
    /// </summary>
    /// <param name="name">The configuration section name.</param>
    /// <param name="options">The configuration options to validate.</param>
    /// <returns>Validation result.</returns>
    public ValidateOptionsResult Validate(string? name, ObsidianVaultConfiguration options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.RawVaultPath))
            errors.Add("ObsidianVault RawVaultPath is required");

        if (string.IsNullOrWhiteSpace(options.PristineVaultPath))
            errors.Add("ObsidianVault PristineVaultPath is required");

        if (options.FileExtensions == null || options.FileExtensions.Length == 0)
            errors.Add("ObsidianVault FileExtensions must contain at least one extension");

        if (options.ExcludePatterns == null)
            errors.Add("ObsidianVault ExcludePatterns cannot be null");

        return errors.Count > 0 
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}