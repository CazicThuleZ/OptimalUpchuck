#nullable enable

using OptimalUpchuck.Domain.Events;
using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Domain.Entities;

/// <summary>
/// Represents an elevation proposal created by an agent for human review
/// </summary>
public class ElevationProposal
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>Unique identifier for the elevation proposal</summary>
    public Guid Id { get; private set; }
    /// <summary>Path to the source markdown file that generated this proposal</summary>
    public string SourceFilePath { get; private set; } = string.Empty;
    /// <summary>Type of agent that created this proposal</summary>
    public string AgentType { get; private set; } = string.Empty;
    /// <summary>Original content extracted from the source file</summary>
    public string OriginalContent { get; private set; } = string.Empty;
    /// <summary>Agent-processed and improved content</summary>
    public string CuratedContent { get; private set; } = string.Empty;
    /// <summary>Agent's confidence score for this proposal</summary>
    public ConfidenceScore ConfidenceScore { get; private set; }
    /// <summary>Agent's explanation for why this content was selected</summary>
    public string AgentRationale { get; private set; } = string.Empty;
    /// <summary>Current review status of the proposal</summary>
    public ReviewStatus ReviewStatus { get; private set; }
    /// <summary>When the proposal was created (UTC)</summary>
    public DateTime CreatedAt { get; private set; }
    /// <summary>When the proposal was reviewed (UTC)</summary>
    public DateTime? ReviewedAt { get; private set; }
    /// <summary>Optional comments from the human reviewer</summary>
    public string? ReviewerComments { get; private set; }
    /// <summary>Target location where approved content should be written</summary>
    public string OutputDestination { get; private set; } = string.Empty;
    /// <summary>Foreign key to the agent configuration that created this proposal</summary>
    public Guid AgentConfigurationId { get; private set; }
    /// <summary>JSON metadata about the processing that created this proposal</summary>
    public string ProcessingMetadata { get; private set; } = "{}";

    /// <summary>Navigation property to the agent configuration</summary>
    public AgentConfiguration? AgentConfiguration { get; private set; }

    /// <summary>Domain events raised by this entity</summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Private constructor for EF Core
    private ElevationProposal() { }

    /// <summary>
    /// Creates a new elevation proposal
    /// </summary>
    /// <param name="sourceFilePath">Path to the source file</param>
    /// <param name="agentType">Type of agent creating the proposal</param>
    /// <param name="originalContent">Original content from the source</param>
    /// <param name="curatedContent">Agent-processed content</param>
    /// <param name="confidenceScore">Agent's confidence in the proposal</param>
    /// <param name="agentRationale">Agent's reasoning</param>
    /// <param name="outputDestination">Where to write approved content</param>
    /// <param name="agentConfigurationId">Agent configuration reference</param>
    /// <param name="processingMetadata">Optional processing metadata JSON</param>
    public ElevationProposal(
        string sourceFilePath,
        string agentType,
        string originalContent,
        string curatedContent,
        ConfidenceScore confidenceScore,
        string agentRationale,
        string outputDestination,
        Guid agentConfigurationId,
        string processingMetadata = "{}")
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
            throw new ArgumentException("Source file path cannot be empty", nameof(sourceFilePath));
        if (string.IsNullOrWhiteSpace(agentType))
            throw new ArgumentException("Agent type cannot be empty", nameof(agentType));
        if (string.IsNullOrWhiteSpace(originalContent))
            throw new ArgumentException("Original content cannot be empty", nameof(originalContent));
        if (string.IsNullOrWhiteSpace(curatedContent))
            throw new ArgumentException("Curated content cannot be empty", nameof(curatedContent));
        if (string.IsNullOrWhiteSpace(agentRationale))
            throw new ArgumentException("Agent rationale cannot be empty", nameof(agentRationale));
        if (string.IsNullOrWhiteSpace(outputDestination))
            throw new ArgumentException("Output destination cannot be empty", nameof(outputDestination));
        if (agentConfigurationId == Guid.Empty)
            throw new ArgumentException("Agent configuration ID cannot be empty", nameof(agentConfigurationId));

        Id = Guid.NewGuid();
        SourceFilePath = sourceFilePath;
        AgentType = agentType;
        OriginalContent = originalContent;
        CuratedContent = curatedContent;
        ConfidenceScore = confidenceScore;
        AgentRationale = agentRationale;
        ReviewStatus = ReviewStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        OutputDestination = outputDestination;
        AgentConfigurationId = agentConfigurationId;
        ProcessingMetadata = processingMetadata;

        _domainEvents.Add(new ProposalCreatedEvent(Id, AgentType, SourceFilePath, ConfidenceScore, CreatedAt));
    }

    /// <summary>
    /// Approves the proposal for publication
    /// </summary>
    /// <param name="reviewerComments">Optional reviewer comments</param>
    /// <exception cref="InvalidOperationException">Thrown when proposal is not in Pending status</exception>
    public void Approve(string? reviewerComments = null)
    {
        if (ReviewStatus != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot approve proposal with status {ReviewStatus}");

        ReviewStatus = ReviewStatus.Approved;
        ReviewedAt = DateTime.UtcNow;
        ReviewerComments = reviewerComments;

        _domainEvents.Add(new ProposalApprovedEvent(Id, AgentType, OutputDestination, ReviewerComments, ReviewedAt.Value));
    }

    /// <summary>
    /// Denies the proposal, preventing publication
    /// </summary>
    /// <param name="reviewerComments">Optional reviewer comments</param>
    /// <exception cref="InvalidOperationException">Thrown when proposal is not in Pending status</exception>
    public void Deny(string? reviewerComments = null)
    {
        if (ReviewStatus != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot deny proposal with status {ReviewStatus}");

        ReviewStatus = ReviewStatus.Denied;
        ReviewedAt = DateTime.UtcNow;
        ReviewerComments = reviewerComments;
    }

    /// <summary>
    /// Expires the proposal due to timeout
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when proposal is not in Pending status</exception>
    public void Expire()
    {
        if (ReviewStatus != ReviewStatus.Pending)
            throw new InvalidOperationException($"Cannot expire proposal with status {ReviewStatus}");

        ReviewStatus = ReviewStatus.Expired;
        ReviewedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears all domain events from this entity
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}