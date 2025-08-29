# Optimal Upchuck

An intelligent content curation system for Obsidian journals that uses AI agents to automatically identify and extract high-value content from large personal journal collections.

## Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/downloads)
- [Ollama](https://ollama.ai/) running locally on port 11434

### Initial Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd OptimalUpchuck
   ```

2. **Start infrastructure services**
   ```bash
   docker-compose up -d
   ```

3. **Restore .NET dependencies**
   ```bash
   dotnet restore OptimalUpchuck.sln
   ```

4. **Run database migrations**
   ```bash
   dotnet ef database update --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui
   ```

5. **Start the application**
   ```bash
   dotnet run --project src/OptimalUpchuck.Ui
   ```

6. **Access the application**
   - Web UI: http://localhost:5000
   - Health Check: http://localhost:5000/health

## Project Structure

```
OptimalUpchuck/
├── src/
│   ├── OptimalUpchuck.Domain/          # Core business entities and logic
│   ├── OptimalUpchuck.Application/     # Application services and interfaces
│   ├── OptimalUpchuck.Infrastructure/  # Data access and external integrations
│   ├── OptimalUpchuck.Ui/             # ASP.NET Core web application
│   └── Services/
│       ├── FileWatcherService/        # Monitors Obsidian vault for new files
│       └── AgentWorkerService/        # Processes files through AI agents
├── tests/                             # Unit and integration tests
├── docs/                              # Project documentation
├── deployment/                        # Docker and deployment configurations
└── data/                              # Sample vault data (development)
```

## Core Concepts

### Agent System
- **Statistics Agent**: High-autonomy agent that extracts mood ratings with 95%+ confidence
- **Blogging Agent**: Low-autonomy agent that identifies blog-worthy content for human review

### Data Flow
1. FileWatcherService detects new .md files in raw Obsidian vault
2. File processing message published to RabbitMQ
3. AgentWorkerService consumes message and runs AI agents
4. High-confidence agents commit directly to database
5. Low-confidence agents create elevation proposals for human review
6. Approved proposals output to pristine Obsidian vault

## Development

### Common Commands

```bash
# Build the solution
dotnet build OptimalUpchuck.sln

# Run tests
dotnet test

# Start development environment
docker-compose up -d

# Create a new migration
dotnet ef migrations add <MigrationName> --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui

# Start file watcher service
dotnet run --project src/Services/FileWatcherService

# Start agent worker service
dotnet run --project src/Services/AgentWorkerService
```

### Configuration

Key configuration files:
- `src/OptimalUpchuck.Ui/appsettings.json` - Main application settings
- `src/OptimalUpchuck.Ui/appsettings.Development.json` - Development overrides
- `docker-compose.yml` - Development infrastructure
- `deployment/docker-compose.prod.yml` - Production deployment

### Obsidian Vault Setup

1. **Create vault directories**
   ```bash
   mkdir -p data/raw-vault data/pristine-vault
   ```

2. **Configure vault paths in appsettings.json**
   ```json
   {
     "ObsidianVault": {
       "RawPath": "./data/raw-vault",
       "PristinePath": "./data/pristine-vault"
     }
   }
   ```

3. **Add sample journal entries**
   - Place markdown files in `data/raw-vault/`
   - Include frontmatter with mood ratings: `mood: 4/5`

### Ollama Setup

1. **Install Ollama** from https://ollama.ai/

2. **Pull required model**
   ```bash
   ollama pull mistral-small
   ```

3. **Verify Ollama is running**
   ```bash
   curl http://localhost:11434/api/tags
   ```

## Documentation

Comprehensive documentation is available in the `docs/` directory:

- [Product Requirements](docs/prd/index.md) - Goals, requirements, and epic definitions
- [Architecture](docs/architecture/index.md) - Technical architecture and design decisions
- [Coding Standards](docs/architecture/coding-standards.md) - Development guidelines
- [Tech Stack](docs/architecture/tech-stack.md) - Technology choices and rationale

## Database

The application uses PostgreSQL with Entity Framework Core:

- **Connection String**: Configured in appsettings.json
- **Migrations**: Located in `src/OptimalUpchuck.Infrastructure/Migrations/`
- **Key Tables**: ElevationProposals, ExtractedData, AgentConfigurations

### Database Operations

```bash
# Create migration
dotnet ef migrations add <MigrationName> --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui

# Update database
dotnet ef database update --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui

# Drop database (development only)
dotnet ef database drop --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui
```

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/OptimalUpchuck.Domain.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

- **Unit Tests**: Domain logic and business rules
- **Integration Tests**: Database operations and agent pipeline
- **Test Containers**: PostgreSQL integration testing with Testcontainers

## Deployment

### Development
```bash
docker-compose up -d
```

### Production
```bash
docker-compose -f deployment/docker-compose.prod.yml up -d
```

### Environment Variables

Required for production deployment:

```bash
# Database
POSTGRES_DB=optimalupchuck_prod
POSTGRES_USER=your_db_user
POSTGRES_PASSWORD=your_secure_password

# RabbitMQ
RABBITMQ_USER=your_mq_user
RABBITMQ_PASSWORD=your_secure_password

# Vault Paths
RAW_VAULT_HOST_PATH=/path/to/raw/vault
PRISTINE_VAULT_HOST_PATH=/path/to/pristine/vault

# Version
VERSION_TAG=latest
```

## Monitoring

### Health Checks
- Application: http://localhost:5000/health
- Database connectivity
- RabbitMQ connectivity
- Loaded agent configurations

### Logs
- Structured logging with Serilog
- Development: Console output
- Production: File and structured logging

### RabbitMQ Management
- UI: http://localhost:15672
- Default credentials: admin/dev_password (development)

## Contributing

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Follow coding standards** (see docs/architecture/coding-standards.md)
4. **Write tests** for new functionality
5. **Update documentation** as needed
6. **Submit a pull request**

### Development Workflow

1. **Start infrastructure**: `docker-compose up -d`
2. **Run migrations**: `dotnet ef database update --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui`
3. **Start application**: `dotnet run --project src/OptimalUpchuck.Ui`
4. **Start services** (in separate terminals):
   - File Watcher: `dotnet run --project src/Services/FileWatcherService`
   - Agent Worker: `dotnet run --project src/Services/AgentWorkerService`

## Troubleshooting

### Common Issues

**Ollama Connection Failed**
- Ensure Ollama is running: `ollama serve`
- Check port 11434 is accessible
- Verify model is pulled: `ollama list`

**Database Connection Issues**
- Ensure PostgreSQL container is running: `docker-compose ps`
- Check connection string in appsettings.json
- Verify migrations are applied

**RabbitMQ Connection Issues**
- Check RabbitMQ container status: `docker-compose logs rabbitmq`
- Verify management UI: http://localhost:15672
- Check queue configuration

**File Processing Not Working**
- Verify vault paths in configuration
- Check file permissions on vault directories
- Review FileWatcherService logs

### Getting Help

- Check the [documentation](docs/) for detailed information
- Review [coding standards](docs/architecture/coding-standards.md)
- Examine the [architecture documentation](docs/architecture/index.md)

## License

[Add your license information here]

## Roadmap

- ✅ Core agent framework
- ✅ Statistics agent implementation
- ✅ Review queue system
- ✅ Blogging agent implementation
- 🔄 Enhanced monitoring and analytics
- 📋 Additional agent types
- 📋 Advanced content categorization
- 📋 Backlog processing (5,000+ existing notes)