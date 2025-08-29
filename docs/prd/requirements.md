# Requirements

## Functional

FR1: The system shall monitor the raw Obsidian vault for new markdown files and automatically queue them for processing via RabbitMQ

FR2: The system shall support configurable agent modules using Microsoft Semantic Kernel framework for specialized content analysis

FR3: The system shall implement a Statistics Agent that autonomously extracts mood ratings and commits them directly to PostgreSQL database

FR4: The system shall implement a Blogging Agent that identifies blog-worthy content and submits elevation proposals to the review queue

FR5: The system shall provide a web-based review queue interface using ASP.NET Razor Pages for human gatekeeper approval/denial of elevation proposals

FR6: The system shall generate elevation proposals containing curated content, confidence scores, and agent rationale

FR7: The system shall commit approved content to designated output destinations (pristine Obsidian vault, PostgreSQL database)

FR8: The system shall maintain consistent frontmatter/metadata structures within each vault (input vs output) and preserve markdown formatting, where output frontmatter may differ from input to support curation workflow requirements

FR9: The system shall support configurable agent autonomy levels (review required vs. autonomous processing)

## Non Functional

NFR1: An always available internet connection is assumed, however the system must be able to operate completely offline using locally hosted AI models

NFR2: Intake pipeline processing must complete within 24 hours of new file detection for daily workflow integration.  Approval queues can accumulate indefinitly 

NFR3: The system shall achieve 90%+ curation accuracy for high-value content identification

NFR4: The system shall reduce content volume to ~10% while preserving ~90% of value in pristine output

NFR5: All external dependencies must use permissive licenses for self-hosting compliance