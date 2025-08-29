# REST API Spec

```yaml
openapi: 3.0.0
info:
  title: Optimal Upchuck Internal API
  version: 1.0.0
  description: Internal API for review queue management and agent configuration
servers:
  - url: http://localhost:5000/api
    description: Local development server

paths:
  /proposals:
    get:
      summary: Get elevation proposals with filtering
      parameters:
        - name: status
          in: query
          schema:
            type: string
            enum: [Pending, Approved, Denied, Expired]
        - name: agentType
          in: query
          schema:
            type: string
        - name: minConfidence
          in: query
          schema:
            type: number
            format: decimal
        - name: maxConfidence
          in: query
          schema:
            type: number
            format: decimal
        - name: dateFrom
          in: query
          schema:
            type: string
            format: date-time
        - name: dateTo
          in: query
          schema:
            type: string
            format: date-time
        - name: search
          in: query
          schema:
            type: string
        - name: page
          in: query
          schema:
            type: integer
            default: 1
        - name: pageSize
          in: query
          schema:
            type: integer
            default: 25
      responses:
        '200':
          description: Filtered proposal list
          content:
            application/json:
              schema:
                type: object
                properties:
                  proposals:
                    type: array
                    items:
                      $ref: '#/components/schemas/ElevationProposalSummary'
                  totalCount:
                    type: integer
                  currentPage:
                    type: integer
                  totalPages:
                    type: integer

  /proposals/{proposalId}:
    get:
      summary: Get detailed proposal for editing
      parameters:
        - name: proposalId
          in: path
          required: true
          schema:
            type: string
            format: uuid
      responses:
        '200':
          description: Detailed proposal
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/ElevationProposalDetail'
        '404':
          description: Proposal not found

    put:
      summary: Update proposal content during review
      parameters:
        - name: proposalId
          in: path
          required: true
          schema:
            type: string
            format: uuid
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                curatedContent:
                  type: string
                reviewerComments:
                  type: string
      responses:
        '200':
          description: Proposal updated successfully
        '400':
          description: Invalid content format
        '404':
          description: Proposal not found

  /proposals/{proposalId}/approve:
    post:
      summary: Approve proposal and trigger output processing
      parameters:
        - name: proposalId
          in: path
          required: true
          schema:
            type: string
            format: uuid
      requestBody:
        required: false
        content:
          application/json:
            schema:
              type: object
              properties:
                reviewerComments:
                  type: string
                outputDestination:
                  type: string
      responses:
        '200':
          description: Proposal approved and processed
          content:
            application/json:
              schema:
                type: object
                properties:
                  success:
                    type: boolean
                  outputFilePath:
                    type: string
                  processedAt:
                    type: string
                    format: date-time
        '400':
          description: Approval failed
        '404':
          description: Proposal not found

  /proposals/{proposalId}/deny:
    post:
      summary: Deny proposal with optional reason
      parameters:
        - name: proposalId
          in: path
          required: true
          schema:
            type: string
            format: uuid
      requestBody:
        required: false
        content:
          application/json:
            schema:
              type: object
              properties:
                reviewerComments:
                  type: string
                denialReason:
                  type: string
      responses:
        '200':
          description: Proposal denied successfully
        '404':
          description: Proposal not found

  /proposals/bulk-actions:
    post:
      summary: Perform bulk operations on multiple proposals
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                proposalIds:
                  type: array
                  items:
                    type: string
                    format: uuid
                action:
                  type: string
                  enum: [approve, deny]
                reviewerComments:
                  type: string
      responses:
        '200':
          description: Bulk operation completed
          content:
            application/json:
              schema:
                type: object
                properties:
                  successCount:
                    type: integer
                  failedCount:
                    type: integer
                  errors:
                    type: array
                    items:
                      type: string

  /agents:
    get:
      summary: Get all agent configurations
      responses:
        '200':
          description: Agent configuration list
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/AgentConfiguration'

  /agents/{agentType}:
    get:
      summary: Get specific agent configuration
      parameters:
        - name: agentType
          in: path
          required: true
          schema:
            type: string
      responses:
        '200':
          description: Agent configuration
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/AgentConfiguration'
        '404':
          description: Agent not found

    put:
      summary: Update agent configuration
      parameters:
        - name: agentType
          in: path
          required: true
          schema:
            type: string
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/AgentConfigurationUpdate'
      responses:
        '200':
          description: Configuration updated successfully
        '400':
          description: Invalid configuration
        '404':
          description: Agent not found

  /agents/{agentType}/toggle:
    post:
      summary: Enable or disable agent
      parameters:
        - name: agentType
          in: path
          required: true
          schema:
            type: string
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
              properties:
                enabled:
                  type: boolean
      responses:
        '200':
          description: Agent status updated
        '404':
          description: Agent not found

  /system/health:
    get:
      summary: System health check
      responses:
        '200':
          description: System health status
          content:
            application/json:
              schema:
                type: object
                properties:
                  status:
                    type: string
                    enum: [Healthy, Degraded, Unhealthy]
                  checks:
                    type: object
                    properties:
                      database:
                        type: string
                      rabbitmq:
                        type: string
                      ollama:
                        type: string
                      fileWatcher:
                        type: string

  /system/statistics:
    get:
      summary: Get processing statistics
      responses:
        '200':
          description: System statistics
          content:
            application/json:
              schema:
                type: object
                properties:
                  pendingProposals:
                    type: integer
                  approvedToday:
                    type: integer
                  deniedToday:
                    type: integer
                  averageProcessingTime:
                    type: number
                  queueDepth:
                    type: integer

components:
  schemas:
    ElevationProposalSummary:
      type: object
      properties:
        id:
          type: string
          format: uuid
        agentType:
          type: string
        sourceFilePath:
          type: string
        contentPreview:
          type: string
          maxLength: 200
        confidenceScore:
          type: number
          format: decimal
        createdAt:
          type: string
          format: date-time
        reviewStatus:
          type: string
          enum: [Pending, Approved, Denied, Expired]

    ElevationProposalDetail:
      type: object
      properties:
        id:
          type: string
          format: uuid
        agentType:
          type: string
        sourceFilePath:
          type: string
        originalContent:
          type: string
        curatedContent:
          type: string
        confidenceScore:
          type: number
          format: decimal
        agentRationale:
          type: string
        reviewStatus:
          type: string
        createdAt:
          type: string
          format: date-time
        reviewerComments:
          type: string
        outputDestination:
          type: string

    AgentConfiguration:
      type: object
      properties:
        id:
          type: string
          format: uuid
        agentType:
          type: string
        isEnabled:
          type: boolean
        autonomyLevel:
          type: string
          enum: [ReviewRequired, SemiAutonomous, FullyAutonomous]
        confidenceThreshold:
          type: number
          format: decimal
        configurationJson:
          type: string
        modelParameters:
          type: string
        version:
          type: integer
        updatedAt:
          type: string
          format: date-time

    AgentConfigurationUpdate:
      type: object
      properties:
        isEnabled:
          type: boolean
        autonomyLevel:
          type: string
          enum: [ReviewRequired, SemiAutonomous, FullyAutonomous]
        confidenceThreshold:
          type: number
          format: decimal
        configurationJson:
          type: string
        modelParameters:
          type: string
```
