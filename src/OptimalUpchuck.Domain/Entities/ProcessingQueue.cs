#nullable enable

namespace OptimalUpchuck.Domain.Entities;

/// <summary>
/// Represents a message in the processing queue for tracking file processing status
/// </summary>
public class ProcessingQueue
{
    public Guid Id { get; private set; }
    public string FilePath { get; private set; } = string.Empty;
    public string MessageId { get; private set; } = string.Empty;
    public ProcessingStatus Status { get; private set; }
    public DateTime QueuedAt { get; private set; }
    public DateTime? ProcessingStartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }
    public string ProcessingMetadata { get; private set; } = "{}";

    // Private constructor for EF Core
    private ProcessingQueue() { }

    public ProcessingQueue(string filePath, string messageId, string processingMetadata = "{}")
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));
        if (string.IsNullOrWhiteSpace(messageId))
            throw new ArgumentException("Message ID cannot be empty", nameof(messageId));

        Id = Guid.NewGuid();
        FilePath = filePath;
        MessageId = messageId;
        Status = ProcessingStatus.Queued;
        QueuedAt = DateTime.UtcNow;
        RetryCount = 0;
        ProcessingMetadata = processingMetadata;
    }

    public void StartProcessing()
    {
        if (Status != ProcessingStatus.Queued && Status != ProcessingStatus.Failed)
            throw new InvalidOperationException($"Cannot start processing from status {Status}");

        Status = ProcessingStatus.Processing;
        ProcessingStartedAt = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void CompleteProcessing()
    {
        if (Status != ProcessingStatus.Processing)
            throw new InvalidOperationException($"Cannot complete processing from status {Status}");

        Status = ProcessingStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void FailProcessing(string errorMessage)
    {
        if (Status != ProcessingStatus.Processing)
            throw new InvalidOperationException($"Cannot fail processing from status {Status}");

        Status = ProcessingStatus.Failed;
        CompletedAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public bool CanRetry(int maxRetryCount = 3) => Status == ProcessingStatus.Failed && RetryCount < maxRetryCount;

    public void Retry()
    {
        if (!CanRetry())
            throw new InvalidOperationException("Cannot retry processing");

        Status = ProcessingStatus.Queued;
        ErrorMessage = null;
    }
}

/// <summary>
/// Represents the processing status of a queued message
/// </summary>
public enum ProcessingStatus
{
    Queued = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}