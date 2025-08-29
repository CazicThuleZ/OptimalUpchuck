# Coding Standards

### Core Standards

- **Languages & Runtimes:** C# 12 with .NET 8.0, target framework net8.0
- **Style & Linting:** EditorConfig with mandatory formatting rules, treat warnings as errors in Release builds
- **Test Organization:** Test files mirror source structure with `.Tests` suffix, one test class per source class

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| **Services** | Interface with `I` prefix, implementation without | `IAgentOrchestrationService`, `AgentOrchestrationService` |
| **Agents** | Suffix with `Agent` | `StatisticsAgent`, `BloggingAgent` |
| **Controllers** | Suffix with `Controller` | `ReviewQueueController` |
| **Database Entities** | Pascal case, no suffixes | `ElevationProposal`, `AgentConfiguration` |
| **DTOs** | Suffix with `Dto` | `ProposalSummaryDto`, `AgentConfigurationDto` |

### Critical Rules

- **Dependency Injection Registration:** All services MUST be registered in `Program.cs` with appropriate lifetimes - Singleton for stateless services, Scoped for database contexts, Transient for lightweight operations
- **Entity Framework Context Usage:** NEVER use DbContext directly in domain layer - always access through repository interfaces in Application layer
- **Error Handling Consistency:** All public methods MUST handle exceptions and log with correlation IDs - use structured logging with Serilog and include agent type, operation, and correlation ID
- **Configuration Access:** Access configuration ONLY through IOptions pattern - never use IConfiguration directly in business logic, always bind to strongly-typed configuration classes
- **Async/Await Pattern:** ALL database and HTTP operations MUST use async/await with proper ConfigureAwait(false) - avoid blocking async calls with .Result or .Wait()
- **Clean Architecture Boundaries:** Domain layer MUST NOT reference Application, Infrastructure, or Presentation layers - enforce with ArchUnit tests if possible
- **Agent Interface Compliance:** All agents MUST implement IAgent interface and return structured AgentResult objects - never return raw strings or unstructured data
- **Database Transactions:** Multi-step operations MUST use explicit transactions through Unit of Work pattern - particularly for elevation proposal creation and approval workflows
- **Correlation ID Propagation:** ALL service calls MUST propagate correlation IDs through HTTP headers and logging context - use Activity.Current.Id or generate new GUID

### Language-Specific Guidelines

#### C# Specifics

- **Record Types:** Use record types for DTOs and value objects that require immutability - prefer `public record ProposalSummaryDto(Guid Id, string AgentType, decimal ConfidenceScore)`
- **Nullable Reference Types:** Enable nullable reference types project-wide and handle all nullable warnings - use `#nullable enable` and proper null-conditional operators
- **Pattern Matching:** Use pattern matching for exception handling in middleware and agent result processing - prefer switch expressions over if-else chains
- **ConfigureAwait:** Always use `ConfigureAwait(false)` for library code to avoid deadlocks - particularly critical in agent processing loops
- **Dispose Pattern:** Implement IDisposable for classes that manage unmanaged resources - especially HTTP clients and file watchers
