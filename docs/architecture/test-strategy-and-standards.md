# Test Strategy and Standards

### Testing Philosophy

- **Approach:** Test-driven development for domain logic, behavior-driven development for agent workflows
- **Coverage Goals:** 80% code coverage minimum, 95% for domain layer, 70% for infrastructure layer
- **Test Pyramid:** 60% unit tests, 30% integration tests, 10% end-to-end tests

### Test Types and Organization

#### Unit Tests for Clean Architecture Layers

- **Framework:** xUnit 2.6.1 with FluentAssertions for readable assertions
- **File Convention:** `{ClassName}Tests.cs` in parallel folder structure
- **Location:** `tests/{LayerName}.Tests/` matching source project structure
- **Mocking Library:** Moq 4.20.69 for interface mocking and behavior verification
- **Coverage Requirement:** 95% for Domain layer, 85% for Application layer

**AI Agent Requirements:**
- Generate tests for all public methods and agent interfaces
- Cover edge cases including malformed content, confidence score boundaries, and configuration edge cases
- Follow AAA pattern (Arrange, Act, Assert) with clear test method naming
- Mock all external dependencies including Ollama API, database repositories, and file system operations
- Test agent-specific error conditions and timeout scenarios

```csharp
[Fact]
public async Task StatisticsAgent_ExtractMoodRating_ReturnsHighConfidenceForValidPattern()
{
    // Arrange
    var mockSemanticKernel = new Mock<ISemanticKernelService>();
    var configuration = new AgentConfiguration 
    { 
        AgentType = "StatisticsAgent", 
        ConfidenceThreshold = 0.95m 
    };
    var content = "Today was great! mood: 4/5 and I feel energetic.";
    
    mockSemanticKernel
        .Setup(x => x.ProcessContentAsync(It.IsAny<string>(), It.IsAny<AgentConfiguration>()))
        .ReturnsAsync(new AgentResult 
        { 
            ConfidenceScore = 0.98m, 
            ExtractedData = "4/5" 
        });
    
    var agent = new StatisticsAgent(mockSemanticKernel.Object);
    
    // Act
    var result = await agent.ProcessAsync(content, configuration);
    
    // Assert
    result.ConfidenceScore.Should().BeGreaterThan(0.95m);
    result.ShouldBypassReview.Should().BeTrue();
    result.ExtractedData.Should().Contain("mood", "4");
}
```

#### Integration Tests

- **Scope:** Cross-layer interactions, database operations, agent pipeline workflows
- **Location:** `tests/OptimalUpchuck.Integration.Tests/`
- **Test Infrastructure:**
  - **PostgreSQL:** Testcontainers with isolated database per test class
  - **RabbitMQ:** Testcontainers with in-memory broker for message testing
  - **Ollama API:** WireMock for stubbing AI model responses

```csharp
public class AgentProcessingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("optimalupchuck_test")
        .Build();
    
    [Fact]
    public async Task EndToEndProcessing_NewFileDetected_CreatesElevationProposal()
    {
        // Arrange - Real database, mocked Ollama
        await _dbContainer.StartAsync();
        var connectionString = _dbContainer.GetConnectionString();
        
        // Setup WireMock for Ollama API
        var wireMockServer = WireMockServer.Start();
        wireMockServer
            .Given(Request.Create().WithPath("/api/generate").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("content-type", "application/json")
                .WithBody(JsonSerializer.Serialize(new { response = "This is blog-worthy content" })));
        
        // Act - Simulate file processing workflow
        var testFile = "test-journal-entry.md";
        await CreateTestMarkdownFile(testFile, "Today I had an interesting insight about clean architecture...");
        
        // Trigger file watcher and wait for processing
        await TriggerFileProcessing(testFile);
        await Task.Delay(5000); // Allow processing time
        
        // Assert - Verify elevation proposal created
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IElevationProposalRepository>();
        var proposals = await repository.GetPendingProposalsAsync();
        
        proposals.Should().HaveCount(1);
        proposals.First().AgentType.Should().Be("BloggingAgent");
        proposals.First().ConfidenceScore.Should().BeInRange(0.6m, 1.0m);
    }
}
```

#### End-to-End Tests

- **Framework:** Playwright for browser automation testing review queue functionality
- **Scope:** Complete user workflows from file creation through approval
- **Environment:** Dockerized test environment with all services running
- **Test Data:** Synthetic journal entries with known patterns for predictable agent responses

```csharp
[Test]
public async Task ReviewQueue_ApproveProposal_GeneratesPristineVaultFile()
{
    // Arrange - Setup test environment with real services
    await StartTestEnvironment();
    await SeedTestData();
    
    // Navigate to review queue
    await Page.GotoAsync("http://localhost:8080/reviewqueue");
    
    // Act - Approve first proposal
    await Page.ClickAsync("[data-testid='proposal-approve-button']:first-child");
    await Page.FillAsync("[data-testid='reviewer-comments']", "Approved for publication");
    await Page.ClickAsync("[data-testid='confirm-approval']");
    
    // Wait for success notification
    await Page.WaitForSelectorAsync(".swal2-success");
    
    // Assert - Verify file created in pristine vault
    var pristineFiles = Directory.GetFiles("/test-data/pristine-vault", "*.md");
    pristineFiles.Should().HaveCountGreaterThan(0);
    
    var approvedContent = await File.ReadAllTextAsync(pristineFiles.First());
    approvedContent.Should().Contain("# Blog Idea");
    approvedContent.Should().Contain("clean architecture");
}
```

### Agent-Specific Testing Strategy

#### Agent Behavior Validation

- **Confidence Score Testing:** Verify agents produce scores within valid ranges (0.0-1.0)
- **Content Pattern Recognition:** Test agent accuracy with known content patterns
- **Error Handling:** Verify graceful degradation when Ollama API is unavailable
- **Configuration Compliance:** Test agent behavior changes with different configuration settings

#### Mock Agent for Deterministic Testing

```csharp
public class MockStatisticsAgent : IAgent
{
    private readonly Dictionary<string, AgentResult> _predefinedResults;
    
    public MockStatisticsAgent()
    {
        _predefinedResults = new Dictionary<string, AgentResult>
        {
            ["mood: 5/5"] = new AgentResult 
            { 
                ConfidenceScore = 0.99m, 
                ShouldBypassReview = true,
                ExtractedData = new { mood = 5, scale = 5 }
            },
            ["feeling great today"] = new AgentResult 
            { 
                ConfidenceScore = 0.45m, 
                ShouldBypassReview = false,
                ExtractedData = new { sentiment = "positive", confidence = "low" }
            }
        };
    }
    
    public Task<AgentResult> ProcessAsync(string content, AgentConfiguration config)
    {
        var matchingPattern = _predefinedResults.Keys
            .FirstOrDefault(pattern => content.Contains(pattern, StringComparison.OrdinalIgnoreCase));
            
        var result = matchingPattern != null 
            ? _predefinedResults[matchingPattern]
            : new AgentResult { ConfidenceScore = 0.1m, ShouldBypassReview = false };
            
        return Task.FromResult(result);
    }
}
```

### Clean Architecture Validation Tests

#### Dependency Rule Enforcement

```csharp
[Fact]
public void Domain_ShouldNotDependOn_ApplicationOrInfrastructure()
{
    var domainAssembly = typeof(ElevationProposal).Assembly;
    var applicationAssembly = typeof(IElevationProposalService).Assembly;
    var infrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    
    var result = Types.InAssembly(domainAssembly)
        .Should()
        .NotHaveDependencyOn(applicationAssembly.GetName().Name)
        .And()
        .NotHaveDependencyOn(infrastructureAssembly.GetName().Name)
        .GetResult();
    
    result.IsSuccessful.Should().BeTrue(result.FailingTypes?.ToString());
}

[Fact]
public void Application_ShouldNotDependOn_Infrastructure()
{
    var applicationAssembly = typeof(IElevationProposalService).Assembly;
    var infrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    
    var result = Types.InAssembly(applicationAssembly)
        .Should()
        .NotHaveDependencyOn(infrastructureAssembly.GetName().Name)
        .GetResult();
    
    result.IsSuccessful.Should().BeTrue(result.FailingTypes?.ToString());
}
```

### Test Data Management

- **Strategy:** Test data builders with fluent interface for complex entity creation
- **Fixtures:** Shared test data sets for common scenarios (valid proposals, agent configurations)
- **Cleanup:** Automatic test database cleanup between test runs using Testcontainers lifecycle

```csharp
public class ElevationProposalBuilder
{
    private ElevationProposal _proposal = new();
    
    public ElevationProposalBuilder WithAgentType(string agentType)
    {
        _proposal.AgentType = agentType;
        return this;
    }
    
    public ElevationProposalBuilder WithConfidenceScore(decimal score)
    {
        _proposal.ConfidenceScore = score;
        return this;
    }
    
    public ElevationProposalBuilder AsBlogProposal()
    {
        return WithAgentType("BloggingAgent")
            .WithConfidenceScore(0.75m)
            .WithContent("This is an interesting blog idea about clean architecture...");
    }
    
    public ElevationProposal Build() => _proposal;
}
```

### Continuous Testing

- **CI Integration:** All tests run on every pull request with parallel execution
- **Performance Tests:** Agent processing time benchmarks to detect performance regressions
- **Security Tests:** SAST integration for dependency vulnerability scanning
- **Contract Tests:** API contract validation using OpenAPI specification