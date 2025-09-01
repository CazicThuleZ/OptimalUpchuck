#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OptimalUpchuck.Domain.Entities;
using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ElevationProposal entity
/// </summary>
public class ElevationProposalConfiguration : IEntityTypeConfiguration<ElevationProposal>
{
    public void Configure(EntityTypeBuilder<ElevationProposal> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Properties with constraints
        builder.Property(e => e.SourceFilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.AgentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.OriginalContent)
            .IsRequired();

        builder.Property(e => e.CuratedContent)
            .IsRequired();

        // ConfidenceScore value object conversion
        builder.Property(e => e.ConfidenceScore)
            .HasConversion(
                v => v.Value,
                v => new ConfidenceScore(v))
            .HasColumnType("decimal(3,2)")
            .IsRequired();

        builder.Property(e => e.AgentRationale)
            .IsRequired();

        // ReviewStatus enum conversion
        builder.Property(e => e.ReviewStatus)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.ReviewedAt);

        builder.Property(e => e.ReviewerComments)
            .HasMaxLength(2000);

        builder.Property(e => e.OutputDestination)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.AgentConfigurationId)
            .IsRequired();

        // JSONB column for processing metadata
        builder.Property(e => e.ProcessingMetadata)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}")
            .IsRequired();

        // Configure table with constraints
        builder.ToTable("ElevationProposals", t =>
        {
            t.HasCheckConstraint("CK_ElevationProposals_ConfidenceScore", 
                "\"ConfidenceScore\" >= 0.0 AND \"ConfidenceScore\" <= 1.0");

            t.HasCheckConstraint("CK_ElevationProposals_ReviewStatus",
                "\"ReviewStatus\" IN ('Pending', 'Approved', 'Denied', 'Expired')");

            // ReviewedAt logic constraint
            t.HasCheckConstraint("CK_ElevationProposals_ReviewedAt_Logic",
                "(\"ReviewStatus\" = 'Pending' AND \"ReviewedAt\" IS NULL) OR " +
                "(\"ReviewStatus\" != 'Pending' AND \"ReviewedAt\" IS NOT NULL)");
        });

        // Indexes for performance
        builder.HasIndex(e => new { e.ReviewStatus, e.CreatedAt })
            .HasDatabaseName("IX_ElevationProposals_Status_CreatedAt");

        builder.HasIndex(e => e.AgentType)
            .HasDatabaseName("IX_ElevationProposals_AgentType");

        builder.HasIndex(e => e.SourceFilePath)
            .HasDatabaseName("IX_ElevationProposals_SourceFilePath");

        // Full-text search index
        builder.HasIndex(e => new { e.CuratedContent, e.AgentRationale })
            .HasDatabaseName("IX_ElevationProposals_FullTextSearch")
            .HasMethod("gin")
            .HasAnnotation("Npgsql:TsVectorConfig", "english");

        // Foreign key relationship
        builder.HasOne(e => e.AgentConfiguration)
            .WithMany(a => a.ElevationProposals)
            .HasForeignKey(e => e.AgentConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events for EF Core
        builder.Ignore(e => e.DomainEvents);
    }
}