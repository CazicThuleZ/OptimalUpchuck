# Technical Assumptions

**Repository Structure:** Monorepo

**Service Architecture:** Microservices architecture with file watcher service, RabbitMQ message queue, and specialized agent worker services within a monorepo structure, following clean architecture principles

**Testing Requirements:** Unit testing with integration testing for agent pipeline workflows and database operations

**Additional Technical Assumptions:**

- **Primary Language & Runtime:** C# with .NET 8 for all services and web interface
- **AI Orchestration Framework:** Microsoft Semantic Kernel for agent module loading and execution
- **Database System:** PostgreSQL for elevation proposals, review queue, and extracted structured data
- **Message Queue:** RabbitMQ for file processing workflow coordination
- **Web Framework:** ASP.NET Core with Razor Pages for review queue interface
- **Containerization:** Docker containers for service deployment on Windows Server
- **File System Integration:** Direct file system access to Obsidian vault directories
- **Configuration Management:** YAML/JSON configuration files for agent settings and autonomy levels
- **Self-Hosting Infrastructure:** Windows Server environment with offline-capable operation
- **Project Naming Convention:** Solution shall use OptimalUpchuck.* namespace with clear layer separation (e.g., OptimalUpchuck.Domain, OptimalUpchuck.Api, OptimalUpchuck.Infrastructure, OptimalUpchuck.Ui)
- **Assembly Organization:** Clean architecture layer separation with distinct projects for domain, application services, infrastructure, and presentation concerns

**AI Model Infrastructure:**
- **Local AI Platform:** Ollama inference server hosted at http://localhost:11434
- **Model Management:** Dynamic model loading/unloading via Ollama API operations
- **Initial Model:** mistral-small (with capability to switch models based on agent requirements and the ever improving model ecosystem)
- **Integration:** Microsoft Semantic Kernel configured to communicate with Ollama HTTP API

**UI Infrastructure:**
- **Existing Foundation:** OptimalUpchuck.Ui project with established menu system and site-level styling
- **Extension Approach:** Extend existing Ui patterns and navigation rather than comprehensive redesign
- **Scope:** Minimal functional interface for elevation proposal review queue operations
- **Design Philosophy:** Functional over aesthetic - leverage existing styles and focus on workflow efficiency