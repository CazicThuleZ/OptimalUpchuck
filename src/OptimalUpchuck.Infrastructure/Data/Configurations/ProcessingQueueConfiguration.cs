#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OptimalUpchuck.Domain.Entities;

namespace OptimalUpchuck.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ProcessingQueue entity
/// </summary>
public class ProcessingQueueConfiguration : IEntityTypeConfiguration<ProcessingQueue>
{
    public void Configure(EntityTypeBuilder<ProcessingQueue> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Properties with constraints
        builder.Property(e => e.FilePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.MessageId)
            .HasMaxLength(100)
            .IsRequired();

        // ProcessingStatus enum conversion
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.QueuedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.ProcessingStartedAt);

        builder.Property(e => e.CompletedAt);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.RetryCount)
            .HasDefaultValue(0)
            .IsRequired();

        // JSONB column for processing metadata
        builder.Property(e => e.ProcessingMetadata)
            .HasColumnType("jsonb")
            .HasDefaultValue("{}")
            .IsRequired();

        // Configure table with constraints
        builder.ToTable("ProcessingQueue", t =>
        {
            t.HasCheckConstraint("CK_ProcessingQueue_Status",
                "\"Status\" IN ('Queued', 'Processing', 'Completed', 'Failed')");

            t.HasCheckConstraint("CK_ProcessingQueue_RetryCount",
                "\"RetryCount\" >= 0");

            // Processing time validation constraint
            t.HasCheckConstraint("CK_ProcessingQueue_ProcessingTime",
                "(\"ProcessingStartedAt\" IS NULL OR \"QueuedAt\" <= \"ProcessingStartedAt\") AND " +
                "(\"CompletedAt\" IS NULL OR \"ProcessingStartedAt\" <= \"CompletedAt\")");
        });

        // Indexes for performance
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_ProcessingQueue_Status");

        builder.HasIndex(e => e.MessageId)
            .IsUnique()
            .HasDatabaseName("IX_ProcessingQueue_MessageId_Unique");

        builder.HasIndex(e => e.FilePath)
            .HasDatabaseName("IX_ProcessingQueue_FilePath");

        builder.HasIndex(e => new { e.Status, e.QueuedAt })
            .HasDatabaseName("IX_ProcessingQueue_Status_QueuedAt");
    }
}