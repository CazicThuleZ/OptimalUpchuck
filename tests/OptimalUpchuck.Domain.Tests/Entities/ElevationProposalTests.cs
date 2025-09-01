#nullable enable

using FluentAssertions;
using OptimalUpchuck.Domain.Entities;
using OptimalUpchuck.Domain.Events;
using OptimalUpchuck.Domain.ValueObjects;
using Xunit;

namespace OptimalUpchuck.Domain.Tests.Entities;

/// <summary>
/// Unit tests for ElevationProposal entity
/// </summary>
public class ElevationProposalTests
{
    private readonly Guid _agentConfigurationId = Guid.NewGuid();
    private const string SourceFilePath = "/test/source.md";
    private const string AgentType = "TestAgent";
    private const string OriginalContent = "Original test content";
    private const string CuratedContent = "Curated test content";
    private const string AgentRationale = "Test rationale";
    private const string OutputDestination = "/test/output.md";
    private readonly ConfidenceScore _confidenceScore = new(0.85m);

    [Fact]
    public void Constructor_WithValidParameters_SetsPropertiesCorrectly()
    {
        // Act
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        // Assert
        proposal.Id.Should().NotBeEmpty();
        proposal.SourceFilePath.Should().Be(SourceFilePath);
        proposal.AgentType.Should().Be(AgentType);
        proposal.OriginalContent.Should().Be(OriginalContent);
        proposal.CuratedContent.Should().Be(CuratedContent);
        proposal.ConfidenceScore.Should().Be(_confidenceScore);
        proposal.AgentRationale.Should().Be(AgentRationale);
        proposal.ReviewStatus.Should().Be(ReviewStatus.Pending);
        proposal.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.ReviewedAt.Should().BeNull();
        proposal.ReviewerComments.Should().BeNull();
        proposal.OutputDestination.Should().Be(OutputDestination);
        proposal.AgentConfigurationId.Should().Be(_agentConfigurationId);
        proposal.ProcessingMetadata.Should().Be("{}");
    }

    [Fact]
    public void Constructor_WithValidParameters_RaisesProposalCreatedEvent()
    {
        // Act
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        // Assert
        proposal.DomainEvents.Should().HaveCount(1);
        var domainEvent = proposal.DomainEvents.First().Should().BeOfType<ProposalCreatedEvent>().Subject;
        domainEvent.ProposalId.Should().Be(proposal.Id);
        domainEvent.AgentType.Should().Be(AgentType);
        domainEvent.SourceFilePath.Should().Be(SourceFilePath);
        domainEvent.ConfidenceScore.Should().Be(_confidenceScore);
        domainEvent.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "AgentType", "OriginalContent", "CuratedContent", "AgentRationale", "OutputDestination")]
    [InlineData("SourceFilePath", "", "OriginalContent", "CuratedContent", "AgentRationale", "OutputDestination")]
    [InlineData("SourceFilePath", "AgentType", "", "CuratedContent", "AgentRationale", "OutputDestination")]
    [InlineData("SourceFilePath", "AgentType", "OriginalContent", "", "AgentRationale", "OutputDestination")]
    [InlineData("SourceFilePath", "AgentType", "OriginalContent", "CuratedContent", "", "OutputDestination")]
    [InlineData("SourceFilePath", "AgentType", "OriginalContent", "CuratedContent", "AgentRationale", "")]
    public void Constructor_WithInvalidParameters_ThrowsArgumentException(
        string sourceFilePath, 
        string agentType, 
        string originalContent, 
        string curatedContent, 
        string agentRationale, 
        string outputDestination)
    {
        // Act & Assert
        var act = () => new ElevationProposal(
            sourceFilePath,
            agentType,
            originalContent,
            curatedContent,
            _confidenceScore,
            agentRationale,
            outputDestination,
            _agentConfigurationId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyAgentConfigurationId_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            Guid.Empty);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Agent configuration ID cannot be empty*");
    }

    [Fact]
    public void Approve_WhenPending_UpdatesStatusAndRaisesEvent()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        const string reviewerComments = "Test approval comments";

        // Act
        proposal.Approve(reviewerComments);

        // Assert
        proposal.ReviewStatus.Should().Be(ReviewStatus.Approved);
        proposal.ReviewedAt.Should().NotBeNull();
        proposal.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.ReviewerComments.Should().Be(reviewerComments);

        proposal.DomainEvents.Should().HaveCount(2);
        var approvalEvent = proposal.DomainEvents.Last().Should().BeOfType<ProposalApprovedEvent>().Subject;
        approvalEvent.ProposalId.Should().Be(proposal.Id);
        approvalEvent.AgentType.Should().Be(AgentType);
        approvalEvent.OutputDestination.Should().Be(OutputDestination);
        approvalEvent.ReviewerComments.Should().Be(reviewerComments);
    }

    [Fact]
    public void Approve_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        proposal.Approve();

        // Act & Assert
        var act = () => proposal.Approve();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot approve proposal with status Approved*");
    }

    [Fact]
    public void Deny_WhenPending_UpdatesStatusCorrectly()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        const string reviewerComments = "Test denial comments";

        // Act
        proposal.Deny(reviewerComments);

        // Assert
        proposal.ReviewStatus.Should().Be(ReviewStatus.Denied);
        proposal.ReviewedAt.Should().NotBeNull();
        proposal.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.ReviewerComments.Should().Be(reviewerComments);
    }

    [Fact]
    public void Deny_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        proposal.Deny();

        // Act & Assert
        var act = () => proposal.Deny();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot deny proposal with status Denied*");
    }

    [Fact]
    public void Expire_WhenPending_UpdatesStatusCorrectly()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        // Act
        proposal.Expire();

        // Assert
        proposal.ReviewStatus.Should().Be(ReviewStatus.Expired);
        proposal.ReviewedAt.Should().NotBeNull();
        proposal.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Expire_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        proposal.Expire();

        // Act & Assert
        var act = () => proposal.Expire();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot expire proposal with status Expired*");
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var proposal = new ElevationProposal(
            SourceFilePath,
            AgentType,
            OriginalContent,
            CuratedContent,
            _confidenceScore,
            AgentRationale,
            OutputDestination,
            _agentConfigurationId);

        proposal.DomainEvents.Should().HaveCount(1);

        // Act
        proposal.ClearDomainEvents();

        // Assert
        proposal.DomainEvents.Should().BeEmpty();
    }
}