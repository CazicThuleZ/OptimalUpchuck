# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Optimal Upchuck is an intelligent content curation system for Obsidian journals. It uses AI agents to automatically identify and extract high-value content from large personal journal collections. The system follows clean architecture principles with a microservices approach within a monorepo structure.

## Common Development Commands

### Frontend Build & Development
```bash
# Navigate to UI project directory
cd src/OptimalUpchuck.Ui

# Install dependencies
npm install

# Development build with file watching
npm run dev

# Production build 
npm run build

# Build RTL version
npm run rtl

# Production RTL build
npm run rtl-build
```

### .NET Core Commands
```bash
# Build entire solution
dotnet build OptimalUpchuck.sln

# Restore packages
dotnet restore OptimalUpchuck.sln

# Run specific project
dotnet run --project src/OptimalUpchuck.Ui

# Run tests
dotnet test

# Create Entity Framework migration
dotnet ef migrations add <MigrationName> --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui

# Update database
dotnet ef database update --project src/OptimalUpchuck.Infrastructure --startup-project src/OptimalUpchuck.Ui
```

### Docker Development
```bash
# Start development environment
docker-compose up -d

# Start production environment  
docker-compose -f deployment/docker-compose.prod.yml up -d

# View logs for specific service
docker-compose logs -f <service-name>

# Rebuild specific service
docker-compose up -d --build <service-name>
```

## Architecture Overview

### Core Components
- **OptimalUpchuck.Ui**: ASP.NET Core web application with Razor Pages for review queue management
- **OptimalUpchuck.Domain**: Domain entities and business logic (ElevationProposal, ExtractedData, AgentConfiguration)
- **OptimalUpchuck.Application**: Application services and interfaces
- **OptimalUpchuck.Infrastructure**: Data access, external APIs, and infrastructure concerns
- **FileWatcherService**: Monitors Obsidian vault for new markdown files
- **AgentWorkerService**: Processes files through AI agents using Microsoft Semantic Kernel

### Key Technologies
- **.NET 8.0**: Primary runtime and framework
- **Microsoft Semantic Kernel**: AI agent orchestration
- **PostgreSQL**: Primary database for proposals and extracted data
- **RabbitMQ**: Message queue for asynchronous file processing
- **Ollama**: Local AI model inference (localhost:11434)
- **Bootstrap & jQuery**: UI framework (already configured)

### Data Flow
1. FileWatcherService detects new .md files in raw Obsidian vault
2. File processing message published to RabbitMQ
3. AgentWorkerService consumes message and runs AI agents
4. High-confidence agents (Statistics) commit directly to database
5. Low-confidence agents (Blogging) create elevation proposals for human review
6. Approved proposals output to pristine Obsidian vault

## Agent System

### Agent Types
- **StatisticsAgent**: High autonomy, extracts mood ratings with 95%+ confidence threshold
- **BloggingAgent**: Low autonomy, identifies blog-worthy content requiring human review

### Agent Configuration
Agent settings stored in database (AgentConfigurations table) with:
- Autonomy levels (ReviewRequired, SemiAutonomous, FullyAutonomous)
- Confidence thresholds
- Model parameters for Semantic Kernel
- Processing rules in JSON format

## Database Schema

### Key Tables
- **ElevationProposals**: Agent recommendations requiring human review
- **ExtractedData**: Autonomous agent outputs (mood ratings, metrics)
- **AgentConfigurations**: Agent settings and behavior rules
- **ProcessingQueue**: RabbitMQ message tracking

### Connection Strings
Database connection configured in appsettings.json under `ConnectionStrings:DefaultConnection`

## File Structure Conventions

### Clean Architecture Layers
- Domain layer contains entities and business rules
- Application layer contains services and use cases  
- Infrastructure layer contains external dependencies
- Presentation layer contains UI and API controllers

### Obsidian Vault Integration
- Raw vault: Input journal entries with original frontmatter
- Pristine vault: Curated output with enhanced frontmatter
- Markdown parsing uses Markdig + YamlDotNet for frontmatter handling

## Development Workflows

### Adding New Agents
1. Create agent class implementing IAgent interface
2. Add configuration entry to AgentConfigurations table
3. Register agent in dependency injection container
4. Update agent factory to handle new agent type

### Review Queue Extensions
- Controllers extend existing OptimalUpchuck.Ui patterns
- Views use established Bootstrap styling and layout structure
- JavaScript uses existing jQuery and DataTables integration

### Testing Strategy
- Unit tests for domain logic and agent behavior
- Integration tests for database operations and agent pipeline
- Use Testcontainers for PostgreSQL in integration tests
- Mock Ollama API responses for deterministic agent testing

## Configuration Management

### Environment-Specific Settings
- Development: appsettings.Development.json
- Production: appsettings.json with environment variables
- Agent configs: Database-stored with versioning support

### Important Settings
- Ollama API URL (default: http://localhost:11434)
- Vault paths for raw and pristine Obsidian directories
- RabbitMQ connection strings
- Database connection strings
- Logging levels and structured logging configuration