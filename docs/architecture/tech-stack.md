# Tech Stack

This is the DEFINITIVE technology selection section. Based on the PRD technical assumptions and existing infrastructure, here are the finalized technology choices:

### Cloud Infrastructure
- **Provider:** Self-hosted Windows Server
- **Key Services:** Docker containerization for PostgreSQL, RabbitMQ, and application services
- **Deployment Regions:** Local datacenter/self-hosted environment

### Technology Stack Table

| Category | Technology | Version | Purpose | Rationale |
|----------|------------|---------|---------|-----------|
| **Language** | C# | .NET 8.0 LTS | Primary development language | Strong typing, excellent tooling, enterprise-ready ecosystem |
| **Runtime** | .NET | 8.0 LTS | Application runtime | Long-term support, performance improvements, self-contained deployments |
| **AI Framework** | Microsoft Semantic Kernel | 1.24.0 | Agent orchestration and AI model abstraction | Native C# integration, Ollama connector support, plugin architecture |
| **Database** | PostgreSQL | 16.1 | Primary data store | ACID compliance, JSON support, excellent Entity Framework integration |
| **ORM** | Entity Framework Core | 8.0 | Data access layer | Code-first migrations, LINQ support, clean architecture compatibility |
| **Message Queue** | RabbitMQ | 3.12 | Async processing coordination | Reliable message delivery, dead letter queues, management UI |
| **Web Framework** | ASP.NET Core | 8.0 | Web UI and API endpoints | Razor Pages for UI, built-in dependency injection, middleware pipeline |
| **Containerization** | Docker | 24.0 | Service orchestration | Consistent deployment, service isolation, development environment parity |
| **Configuration** | Microsoft.Extensions.Configuration | 8.0 | Settings management | JSON/YAML support, environment-specific configs, options pattern |
| **Logging** | Serilog | 3.1.1 | Structured logging | Structured logging, multiple sinks, filtering capabilities |
| **Testing Framework** | xUnit | 2.6.1 | Unit and integration testing | .NET standard, excellent tooling, theory/fact patterns |
| **Mocking** | Moq | 4.20.69 | Test isolation | Interface mocking, behavior verification, clean architecture testing |
| **HTTP Client** | HttpClient | Built-in | Ollama API communication | Native .NET, dependency injection ready, resilience patterns |
| **File Monitoring** | FileSystemWatcher | Built-in | Obsidian vault monitoring | Native Windows file system events, efficient monitoring |
| **Serialization** | System.Text.Json | Built-in | JSON processing | High performance, minimal allocations, modern API |
| **Validation** | FluentValidation | 11.8.0 | Input validation | Fluent interface, complex validation rules, testable |
| **Background Services** | Microsoft.Extensions.Hosting | 8.0 | Worker services | Background task processing, lifecycle management, DI integration |
| **Health Checks** | Microsoft.Extensions.Diagnostics.HealthChecks | 8.0 | Service monitoring | Built-in health check framework, custom checks, UI dashboard |
| **CSS Framework** | Bootstrap | 5.x | UI framework and responsive design | Already installed in existing UI, consistent styling patterns |
| **JavaScript Library** | jQuery | 3.x | DOM manipulation and AJAX | Already installed in existing UI, required for legacy components |
| **Rich Text Editor** | Summernote | Latest | Markdown editing in review queue | Already installed, enables minor content modifications during review |
| **UI Notifications** | Bootstrap Toast | Latest | User feedback and confirmation dialogs | Already installed, provides consistent confirmation UX for approve/deny actions |
| **Data Tables** | DataTables | Latest | Advanced table sorting and filtering | Already installed, supports agent-type filtering and proposal management workflows |
| **Markdown Parser** | Markdig | 0.33.0 | Markdown content parsing and rendering | High performance, extensible, supports advanced markdown features |
| **YAML Parser** | YamlDotNet | 13.7.1 | Frontmatter extraction and manipulation | Robust YAML parsing, handles Obsidian frontmatter structures |
