# Database Schema

```sql
-- Enable UUID extension for primary keys
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Agent Configurations table
CREATE TABLE AgentConfigurations (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    AgentType VARCHAR(100) NOT NULL UNIQUE,
    IsEnabled BOOLEAN NOT NULL DEFAULT true,
    AutonomyLevel VARCHAR(50) NOT NULL CHECK (AutonomyLevel IN ('ReviewRequired', 'SemiAutonomous', 'FullyAutonomous')),
    ConfidenceThreshold DECIMAL(3,2) NOT NULL CHECK (ConfidenceThreshold >= 0.0 AND ConfidenceThreshold <= 1.0),
    ConfigurationJson JSONB,
    ModelParameters JSONB,
    ProcessingRules JSONB,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    Version INTEGER NOT NULL DEFAULT 1
);

-- Elevation Proposals table
CREATE TABLE ElevationProposals (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    SourceFilePath VARCHAR(500) NOT NULL,
    AgentConfigurationId UUID NOT NULL REFERENCES AgentConfigurations(Id),
    AgentType VARCHAR(100) NOT NULL,
    OriginalContent TEXT NOT NULL,
    CuratedContent TEXT NOT NULL,
    ConfidenceScore DECIMAL(3,2) NOT NULL CHECK (ConfidenceScore >= 0.0 AND ConfidenceScore <= 1.0),
    AgentRationale TEXT NOT NULL,
    ReviewStatus VARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (ReviewStatus IN ('Pending', 'Approved', 'Denied', 'Expired')),
    OutputDestination VARCHAR(500),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    ReviewedAt TIMESTAMP WITH TIME ZONE,
    ReviewerComments TEXT,
    ProcessingMetadata JSONB
);

-- Extracted Data table for autonomous agent outputs
CREATE TABLE ExtractedData (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    SourceFilePath VARCHAR(500) NOT NULL,
    AgentConfigurationId UUID NOT NULL REFERENCES AgentConfigurations(Id),
    AgentType VARCHAR(100) NOT NULL,
    DataType VARCHAR(100) NOT NULL,
    DataValue JSONB NOT NULL,
    DataUom VARCHAR(20) NOT NULL,
    ConfidenceScore DECIMAL(3,2) NOT NULL CHECK (ConfidenceScore >= 0.0 AND ConfidenceScore <= 1.0),
    Context TEXT,
    ExtractedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    ProcessingMetadata JSONB
);

-- Processing Queue for RabbitMQ message tracking
CREATE TABLE ProcessingQueue (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    FilePath VARCHAR(500) NOT NULL,
    MessageId VARCHAR(100) UNIQUE,
    QueuedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    ProcessingStartedAt TIMESTAMP WITH TIME ZONE,
    CompletedAt TIMESTAMP WITH TIME ZONE,
    Status VARCHAR(20) NOT NULL DEFAULT 'Queued' CHECK (Status IN ('Queued', 'Processing', 'Completed', 'Failed', 'Retrying')),
    ErrorMessage TEXT,
    RetryCount INTEGER NOT NULL DEFAULT 0,
    ProcessingMetadata JSONB
);

-- Audit Log for tracking all system actions
CREATE TABLE AuditLog (
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    EntityType VARCHAR(100) NOT NULL,
    EntityId UUID NOT NULL,
    Action VARCHAR(50) NOT NULL,
    Changes JSONB,
    UserId VARCHAR(100),
    Timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    IPAddress INET,
    UserAgent TEXT
);

-- Performance indexes
CREATE INDEX IX_ElevationProposals_Status_CreatedAt ON ElevationProposals(ReviewStatus, CreatedAt DESC);
CREATE INDEX IX_ElevationProposals_AgentType ON ElevationProposals(AgentType);
CREATE INDEX IX_ElevationProposals_ConfidenceScore ON ElevationProposals(ConfidenceScore DESC);
CREATE INDEX IX_ElevationProposals_SourceFilePath ON ElevationProposals(SourceFilePath);

CREATE INDEX IX_ExtractedData_DataType_ExtractedAt ON ExtractedData(DataType, ExtractedAt DESC);
CREATE INDEX IX_ExtractedData_AgentType ON ExtractedData(AgentType);
CREATE INDEX IX_ExtractedData_SourceFilePath ON ExtractedData(SourceFilePath);

CREATE INDEX IX_ProcessingQueue_Status_QueuedAt ON ProcessingQueue(Status, QueuedAt);
CREATE INDEX IX_ProcessingQueue_FilePath ON ProcessingQueue(FilePath);

CREATE INDEX IX_AuditLog_EntityType_EntityId ON AuditLog(EntityType, EntityId);
CREATE INDEX IX_AuditLog_Timestamp ON AuditLog(Timestamp DESC);

-- Full-text search index for proposal content
CREATE INDEX IX_ElevationProposals_Content_Search ON ElevationProposals 
USING GIN(to_tsvector('english', CuratedContent || ' ' || AgentRationale));

-- Triggers for UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_agent_configurations_updated_at 
    BEFORE UPDATE ON AgentConfigurations 
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Data retention policies
CREATE OR REPLACE FUNCTION cleanup_old_audit_logs()
RETURNS void AS $$
BEGIN
    DELETE FROM AuditLog WHERE Timestamp < NOW() - INTERVAL '1 year';
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION cleanup_completed_queue_items()
RETURNS void AS $$
BEGIN
    DELETE FROM ProcessingQueue 
    WHERE Status = 'Completed' 
    AND CompletedAt < NOW() - INTERVAL '30 days';
END;
$$ LANGUAGE plpgsql;

-- Constraints for data integrity
ALTER TABLE ElevationProposals 
ADD CONSTRAINT CHK_ReviewedAt_When_Not_Pending 
CHECK (
    (ReviewStatus = 'Pending' AND ReviewedAt IS NULL) OR
    (ReviewStatus != 'Pending' AND ReviewedAt IS NOT NULL)
);

ALTER TABLE ProcessingQueue
ADD CONSTRAINT CHK_Processing_Times
CHECK (
    (ProcessingStartedAt IS NULL OR ProcessingStartedAt >= QueuedAt) AND
    (CompletedAt IS NULL OR CompletedAt >= ProcessingStartedAt)
);

-- Views for common queries
CREATE VIEW PendingProposalsSummary AS
SELECT 
    p.Id,
    p.AgentType,
    p.SourceFilePath,
    LEFT(p.CuratedContent, 200) as ContentPreview,
    p.ConfidenceScore,
    p.CreatedAt,
    p.ReviewStatus,
    ac.AutonomyLevel
FROM ElevationProposals p
JOIN AgentConfigurations ac ON p.AgentConfigurationId = ac.Id
WHERE p.ReviewStatus = 'Pending'
ORDER BY p.ConfidenceScore DESC, p.CreatedAt;

CREATE VIEW AgentProcessingStats AS
SELECT 
    ac.AgentType,
    ac.IsEnabled,
    COUNT(CASE WHEN p.ReviewStatus = 'Pending' THEN 1 END) as PendingCount,
    COUNT(CASE WHEN p.ReviewStatus = 'Approved' THEN 1 END) as ApprovedCount,
    COUNT(CASE WHEN p.ReviewStatus = 'Denied' THEN 1 END) as DeniedCount,
    AVG(p.ConfidenceScore) as AvgConfidence,
    COUNT(ed.Id) as AutonomousExtractions
FROM AgentConfigurations ac
LEFT JOIN ElevationProposals p ON ac.Id = p.AgentConfigurationId
LEFT JOIN ExtractedData ed ON ac.Id = ed.AgentConfigurationId
GROUP BY ac.AgentType, ac.IsEnabled;

-- Seed data for initial agent configurations
INSERT INTO AgentConfigurations (AgentType, AutonomyLevel, ConfidenceThreshold, ConfigurationJson, ModelParameters)
VALUES 
('StatisticsAgent', 'FullyAutonomous', 0.95, 
 '{"patterns": ["mood:\\s*(\\d+(?:\\.\\d+)?)", "energy:\\s*(\\d+(?:\\.\\d+)?)"], "scales": {"mood": {"min": 1, "max": 5}, "energy": {"min": 1, "max": 10}}}',
 '{"model": "mistral-small", "temperature": 0.1, "max_tokens": 100}'),
 
('BloggingAgent', 'ReviewRequired', 0.60,
 '{"criteria": ["unique_insights", "creative_ideas", "technical_learnings"], "min_content_length": 50}',
 '{"model": "mistral-small", "temperature": 0.3, "max_tokens": 500}');
```
