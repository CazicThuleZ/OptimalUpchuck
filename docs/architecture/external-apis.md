# External APIs

### Ollama API

- **Purpose:** Local AI model inference for content analysis through Microsoft Semantic Kernel
- **Documentation:** http://localhost:11434/api (Ollama REST API documentation)
- **Base URL(s):** http://localhost:11434
- **Authentication:** None (localhost trusted environment)
- **Rate Limits:** Hardware-dependent based on local inference capacity

**Key Endpoints Used:**
- `POST /api/generate` - Generate text completions for agent prompts
- `POST /api/chat` - Chat-based interactions for conversational agents
- `GET /api/tags` - List available models for dynamic model management
- `POST /api/pull` - Download/load specific models as needed
- `POST /api/push` - Manage model lifecycle (load/unload for memory optimization)

**Integration Notes:** 
- Microsoft Semantic Kernel configured with Ollama connector for model abstraction
- Dynamic model loading/unloading based on agent requirements and memory constraints
- Retry logic with exponential backoff for model loading delays
- Fallback handling when specific models are unavailable
- Connection pooling for concurrent agent processing

**Fallback Strategies for Service Unavailability:**

1. **Development Environment Fallback:**
   - Mock AI service that returns predefined responses for testing
   - Configurable via `Ollama:UseMockService=true` in appsettings.Development.json
   - Mock responses stored in `config/mock-responses.json` for deterministic testing
   - Allows full development workflow without Ollama dependency

2. **Production Graceful Degradation:**
   - Circuit breaker pattern: Open after 5 consecutive failures, half-open after 30 seconds
   - Queue file processing requests when Ollama unavailable (up to 100 items)
   - Health check endpoint reports Ollama status for monitoring
   - Email alerts to administrators when service down > 12 hours

3. **Model-Specific Fallbacks:**
   - Primary model: `mistral-small` for production inference
   - Fallback model: `phi3` (smaller, faster) for degraded performance mode
   - Emergency fallback: Rule-based processing for Statistics Agent mood extraction
   - Configuration: `Ollama:ModelFallbackChain=["mistral-small", "phi3", "rule-based"]`


### Markdown Processing Libraries

**Markdig Library Integration:**
- **Purpose:** Robust markdown parsing and HTML rendering for Obsidian content processing
- **Documentation:** https://github.com/xoofx/markdig
- **Integration Method:** NuGet package reference with custom pipeline configuration
- **Key Operations:** Parse markdown AST, preserve formatting during agent processing, render to HTML for review queue display

**YamlDotNet Library Integration:**
- **Purpose:** YAML frontmatter extraction and manipulation for Obsidian metadata
- **Documentation:** https://github.com/aaubry/YamlDotNet
- **Integration Method:** NuGet package reference with custom deserializers
- **Key Operations:** Extract frontmatter headers, modify metadata during curation, serialize updated frontmatter

### File System Integration (Obsidian Vaults)

- **Purpose:** Direct file system access to raw and pristine Obsidian vault directories
- **Documentation:** Windows file system APIs, .NET FileSystemWatcher
- **Base Path(s):** Configurable vault locations from appsettings
- **Authentication:** Windows file system permissions

**Key Operations Used:**
- `FileSystemWatcher.Created` - Monitor new markdown file creation
- **Markdig + YamlDotNet Pipeline** - Parse frontmatter and content separately
- `File.WriteAllTextAsync()` - Write curated content with updated frontmatter to pristine vault
- `Directory.GetFiles()` - Batch processing operations

**Integration Notes:**
- **Parsing Pipeline:** YamlDotNet extracts frontmatter → Markdig processes content body → Structured content object for agent analysis
- **Content Preservation:** Markdig AST manipulation maintains markdown structure during agent processing
- **Frontmatter Management:** YamlDotNet handles different frontmatter schemas between raw and pristine vaults
- **Obsidian Compatibility:** Support for wikilinks, tags, and other Obsidian-specific markdown extensions through Markdig configuration
