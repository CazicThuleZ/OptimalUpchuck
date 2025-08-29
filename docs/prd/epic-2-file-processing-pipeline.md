# Epic 2: File Processing Pipeline

**Epic Goal:** Implement automated file detection and message queue processing infrastructure to handle Obsidian markdown files through the curation pipeline.

## Story 2.1: File Watcher Service Implementation

As a system,
I want a file watcher service that monitors the raw Obsidian vault,
so that new journal entries are automatically detected and queued for processing.

**Acceptance Criteria:**
1. FileWatcherService monitors specified directory for new .md files
2. Service ignores temporary files and processes only completed markdown files
3. File detection triggers message publication to RabbitMQ queue
4. Service handles file system events gracefully with proper error handling
5. Configuration allows specifying vault path and file patterns

## Story 2.2: RabbitMQ Message Queue Integration

As a system,
I want RabbitMQ message publishing and consuming capabilities,
so that file processing can be handled asynchronously and reliably.

**Acceptance Criteria:**
1. MessagePublisher can send file processing messages to configured queue
2. Message includes file path, timestamp, and processing metadata
3. Queue configuration supports durable messages and proper routing
4. Connection management handles RabbitMQ service restarts gracefully
5. Basic dead letter queue handling for failed message processing

## Story 2.3: Worker Service & Message Processing

As a system,
I want a worker service that consumes file processing messages,
so that journal entries can be processed by the agent pipeline.

**Acceptance Criteria:**
1. WorkerService consumes messages from RabbitMQ queue
2. Service loads and parses markdown files including frontmatter
3. File content is passed to configured agents for analysis
4. Message processing includes proper acknowledgment and error handling
5. Service can be scaled with multiple worker instances

## Story 2.4: End-to-End File Processing Validation

As a system,
I want end-to-end validation of the file processing pipeline,
so that files flow correctly from detection through message processing.

**Acceptance Criteria:**
1. New markdown file in vault triggers file watcher detection
2. File watcher publishes message to RabbitMQ queue successfully
3. Worker service consumes message and processes file content
4. Processing results are logged with file path and timestamp
5. Pipeline handles multiple concurrent files without data loss