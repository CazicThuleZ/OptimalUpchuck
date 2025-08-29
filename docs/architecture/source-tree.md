# Source Tree

```plaintext
OptimalUpchuck/
├── .github/                           # CI/CD workflows and GitHub configuration
│   └── workflows/
│       ├── ci.yml                     # Continuous integration pipeline
│       └── cd.yml                     # Deployment pipeline
├── docs/                              # Project documentation
│   ├── architecture.md                # This architecture document
│   ├── prd.md                        # Product requirements document
│   ├── api/                          # API documentation
│   │   ├── openapi.yml               # OpenAPI specification
│   │   └── postman-collection.json   # API testing collection
│   └── deployment/                   # Deployment guides
├── src/                              # Source code following clean architecture
│   ├── OptimalUpchuck.Ui/            # Web UI 
│       ├── Program.cs
│       ├── plugins.config.js
│       ├── gulpfile.js
│       ├── Controllers/
│       │   ├── ReviewQueueController.cs
│       │   ├── AgentManagementController.cs
│       │   ├── DashboardsController.cs
│       │   └── HealthController.cs
│       ├── Views/
│       │   ├── _ViewImports.cshtml
│       │   ├── _ViewStart.cshtml
│       │   ├── ReviewQueue/
│       │   │   ├── Index.cshtml
│       │   │   ├── Details.cshtml
│       │   │   └── BulkActions.cshtml
│       │   ├── Dashboards/
│       │   │   ├── Index.cshtml
│       │   └── Shared/
│       │       ├── Partials
│       │       |    ├── _Customizer.cshtml
│       │       |    ├── _Footer.cshtml
│       │       |    ├── _FooterScripts.cshtml
│       │       |    ├── _HeadCSS.cshtml
│       │       |    ├── _PageTitle.cshtml
│       │       |    ├── _SideNav.cshtml
│       │       |    ├── _TitleMeta.cshtml
│       │       |    └── _TopBar.cshtml
│       │       ├── _VerticalLayout.cshtml
│       │       └── _ReviewQueueLayout.cshtml
│       ├── wwwroot/
│       │   ├── css/
│       │   │   ├── app.css       # site level styling
│       │   │   ├── app.min.css   # site level styling minified
│       │   │   └── app.min.css.map
│       │   ├── data/  # Any hardcoded data that may be necessary
│       │   ├── fonts/
│       │   │   ├── tabler-icons.ttf
│       │   │   ├── tabler-icons.woff2
├       |   |   └── tabler-icons.woff
│       │   ├── images/
│       │   │   ├── optimal_upchuck_logo_142x40.png
├       |   |   └── favicon.ico
│       │   ├── plugins/         # external css and js libraries / packages
│       │   |   └──datatables/
│       │   │   |   ├── dataTables.min.js
│       │   │   |   └── responsive.bootstrap5.min.css
│       │   |   └── bootstrap/
│       │   │   |   ├── bootstrap.bundle.min.js
│       │   │   |   └── bootstrap.min.css
│       │   |   └── summernote/
│       │   │       ├── summernote-bs5.min.css
│       │   │       └── summernote-bs5.min.js
│       │   ├── js/
│       │   |   ├── Pages/
│       │   │   |    ├── review-queue.js
│       │   │   |    ├── custom-table.js
│       │   │   |    └── dashboard.js
│       │   │   ├── app.js
│       │   │   ├── vendors.min.js
│       │   │   └── config.js
│       ├── Models/               # View models
│       │   ├── ReviewQueueViewModel.cs
│       │   ├── ErrorViewModel.cs
│       │   └── AgentConfigViewModel.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Dockerfile
│   ├── OptimalUpchuck.Domain/        # Core domain layer
│   │   ├── Entities/                 # Domain entities
│   │   │   ├── ElevationProposal.cs
│   │   │   ├── ExtractedData.cs
│   │   │   ├── AgentConfiguration.cs
│   │   │   └── ProcessingQueue.cs
│   │   ├── ValueObjects/             # Domain value objects
│   │   │   ├── ConfidenceScore.cs
│   │   │   ├── ReviewStatus.cs
│   │   │   └── AutonomyLevel.cs
│   │   ├── Interfaces/               # Domain service interfaces
│   │   │   ├── IAgent.cs
│   │   │   ├── IAgentFactory.cs
│   │   │   ├── IContentProcessor.cs
│   │   │   └── IMarkdownParser.cs
│   │   ├── Events/                   # Domain events
│   │   │   ├── ProposalCreatedEvent.cs
│   │   │   ├── ProposalApprovedEvent.cs
│   │   │   └── DataExtractedEvent.cs
│   │   └── Exceptions/               # Domain-specific exceptions
│   │       ├── AgentProcessingException.cs
│   │       └── InvalidProposalException.cs
│   ├── OptimalUpchuck.Application/   # Application service layer
│   │   ├── Services/                 # Application services
│   │   │   ├── ElevationProposalService.cs
│   │   │   ├── AgentOrchestrationService.cs
│   │   │   ├── FileProcessingService.cs
│   │   │   └── ReviewQueueService.cs
│   │   ├── Interfaces/               # Application interfaces
│   │   │   ├── IElevationProposalService.cs
│   │   │   ├── IAgentOrchestrationService.cs
│   │   │   ├── IFileProcessingService.cs
│   │   │   └── IMessagePublisher.cs
│   │   ├── DTOs/                     # Data transfer objects
│   │   │   ├── ProposalSummaryDto.cs
│   │   │   ├── ProposalDetailDto.cs
│   │   │   └── AgentConfigurationDto.cs
│   │   ├── Validators/               # Input validation
│   │   │   ├── ProposalValidator.cs
│   │   │   └── AgentConfigValidator.cs
│   │   └── Mappings/                 # AutoMapper profiles
│   │       └── DomainProfile.cs
│   ├── OptimalUpchuck.Infrastructure/ # Infrastructure layer
│   │   ├── Data/                     # Database context and repositories
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Repositories/
│   │   │   │   ├── ElevationProposalRepository.cs
│   │   │   │   ├── ExtractedDataRepository.cs
│   │   │   │   └── AgentConfigurationRepository.cs
│   │   │   ├── Configurations/       # Entity Framework configurations
│   │   │   │   ├── ElevationProposalConfiguration.cs
│   │   │   │   └── AgentConfigurationConfiguration.cs
│   │   │   └── Migrations/           # EF Core migrations
│   │   ├── Messaging/                # RabbitMQ implementation
│   │   │   ├── RabbitMQPublisher.cs
│   │   │   ├── RabbitMQConsumer.cs
│   │   │   └── MessageHandlers/
│   │   │       └── FileProcessingHandler.cs
│   │   ├── AI/                       # Semantic Kernel integration
│   │   │   ├── SemanticKernelService.cs
│   │   │   ├── OllamaConnector.cs
│   │   │   ├── Agents/               # Agent implementations
│   │   │   │   ├── StatisticsAgent.cs
│   │   │   │   ├── BloggingAgent.cs
│   │   │   │   └── BaseAgent.cs
│   │   │   └── Prompts/              # Agent prompt templates
│   │   │       ├── StatisticsPrompts.cs
│   │   │       └── BloggingPrompts.cs
│   │   ├── FileSystem/               # File operations
│   │   │   ├── ObsidianVaultService.cs
│   │   │   ├── MarkdownProcessor.cs
│   │   │   └── FileWatcherService.cs
│   │   ├── External/                 # External service clients
│   │   │   └── OllamaApiClient.cs
│   │   └── Configuration/            # Infrastructure configuration
│   │       ├── DatabaseConfiguration.cs
│   │       ├── RabbitMQConfiguration.cs
│   │       └── SemanticKernelConfiguration.cs
│   └── Services/                     # Microservice applications
│       ├── OptimalUpchuck.FileWatcher/ # File monitoring service
│       │   ├── Program.cs
│       │   ├── FileWatcherWorker.cs
│       │   ├── appsettings.json
│       │   ├── appsettings.Development.json
│       │   └── Dockerfile
│       ├── OptimalUpchuck.AgentWorker/ # Agent processing service
│       │   ├── Program.cs
│       │   ├── AgentWorkerService.cs
│       │   ├── appsettings.json
│       │   ├── appsettings.Development.json
│       │   └── Dockerfile
├── tests/                            # Test projects
│   ├── OptimalUpchuck.Domain.Tests/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── Services/
│   ├── OptimalUpchuck.Application.Tests/
│   │   ├── Services/
│   │   ├── Validators/
│   │   └── Integration/
│   ├── OptimalUpchuck.Infrastructure.Tests/
│   │   ├── Data/
│   │   ├── AI/
│   │   ├── Messaging/
│   │   └── FileSystem/
│   └── OptimalUpchuck.Web.Tests/
│       ├── Controllers/
│       ├── Views/
│       └── Integration/
├── scripts/                          # Build and deployment scripts
│   ├── build.sh                     # Cross-platform build script
│   ├── deploy.sh                    # Deployment automation
│   ├── database/
│   │   ├── migrate.sh               # Database migration script
│   │   └── seed.sql                 # Initial data seeding
│   └── docker/
│       └── init-db.sh              # Database initialization for Docker
├── config/                          # Configuration files
│   ├── agents/                      # Agent configuration files
│   │   ├── statistics-agent.yml
│   │   ├── blogging-agent.yml
│   │   └── agent-template.yml
│   ├── environments/                # Environment-specific configs
│   │   ├── development.yml
│   │   ├── staging.yml
│   │   └── production.yml
│   └── logging/
│       └── serilog.json
├── deployment/                      # Infrastructure as Code
│   ├── docker-compose.yml          # Local development environment
│   ├── docker-compose.prod.yml     # Production environment
│   ├── kubernetes/                 # K8s manifests (if needed)
│   │   ├── namespace.yml
│   │   ├── services.yml
│   │   └── deployments.yml
│   └── terraform/                  # Infrastructure provisioning (if cloud)
│       ├── main.tf
│       └── variables.tf
├── .gitignore                       # Git ignore rules
├── .dockerignore                    # Docker ignore rules
├── OptimalUpchuck.sln              # Visual Studio solution file
├── Directory.Build.props            # MSBuild common properties
├── Directory.Build.targets          # MSBuild common targets
├── NuGet.config                     # NuGet package source configuration
├── global.json                      # .NET SDK version specification
└── README.md                       # Project overview and setup instructions
```
