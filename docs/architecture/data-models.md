# Data Models

### ElevationProposal

**Purpose:** Stores agent recommendations for content curation that require human review and approval

**Key Attributes:**
- Id: Guid - Primary key for proposal identification
- SourceFilePath: string - Path to original Obsidian markdown file
- AgentType: enum - Type of agent that created proposal (e.g., "BloggingAgent", "LoglineAgent")
- OriginalContent: string - Raw extracted content from source file
- CuratedContent: string - Agent-processed content with improvements/suggestions
- ConfidenceScore: decimal - Agent's confidence rating (0.0 to 1.0)
- AgentRationale: string - Detailed explanation of why content was selected
- ReviewStatus: enum - Pending, Approved, Denied, Expired
- CreatedAt: DateTime - Proposal creation timestamp (UTC)
- ReviewedAt: DateTime? - Human review timestamp (UTC)
- ReviewerComments: string? - Optional human feedback during review
- OutputDestination: string - Target location for approved content

**Relationships:**
- **With AgentConfiguration:** Many-to-One relationship linking proposals to the agent configuration that created them
- **With SourceFile tracking:** Implicit relationship through SourceFilePath for audit trail
- **With Review workflow:** Self-contained approval workflow within entity

### ExtractedData

**Purpose:** Stores structured data autonomously extracted by high-confidence agents

**Key Attributes:**
- Id: Guid - Primary key for extracted data record
- SourceFilePath: string - Path to original Obsidian markdown file
- AgentType: enum - Type of agent that extracted data
- DataType: string - Category of extracted data (e.g., "MoodRating", "ProductityRating", "ActivityDuration")
- DataValue: string - JSON-serialized extracted value for flexibility
- DataUom: string - Unit of measure that qualifies DataValue (eg. star (as in 1 out of 5 starts), 2 hours, 1 session etc.)
- ConfidenceScore: decimal - Agent's confidence in extraction accuracy
- ExtractedAt: DateTime - Timestamp of extraction (UTC)
- Context: string? - Surrounding text context for validation
- ProcessingMetadata: string? - JSON metadata about extraction process

**Relationships:**
- **With AgentConfiguration:** Many-to-One relationship for tracking extraction patterns
- **With Time-series analysis:** Grouped by DataType and ExtractedAt for trend analysis
- **With Source validation:** Links back to source files for audit purposes

### AgentConfiguration

**Purpose:** Manages agent settings, autonomy levels, and processing rules

**Key Attributes:**
- Id: Guid - Primary key for configuration record
- AgentType: string - Unique identifier for agent type
- IsEnabled: bool - Whether agent is active for processing
- AutonomyLevel: enum - ReviewRequired, SemiAutonomous, FullyAutonomous
- ConfidenceThreshold: decimal - Minimum confidence for autonomous actions
- ConfigurationJson: string - JSON blob for agent-specific settings
- ModelParameters: string? - Semantic Kernel model configuration
- ProcessingRules: string? - JSON rules for content identification
- CreatedAt: DateTime - Configuration creation timestamp
- UpdatedAt: DateTime - Last modification timestamp
- Version: int - Configuration version for change tracking

**Relationships:**
- **With ElevationProposal:** One-to-Many for proposals created under this configuration
- **With ExtractedData:** One-to-Many for autonomous extractions
- **With Agent Framework:** Referenced by Semantic Kernel agent factory for instantiation
