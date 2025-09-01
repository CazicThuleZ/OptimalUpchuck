#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OptimalUpchuck.Domain.Entities;
using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for ExtractedData entity
/// </summary>
public class ExtractedDataConfiguration : IEntityTypeConfiguration<ExtractedData>
{
    public void Configure(EntityTypeBuilder<ExtractedData> builder)
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

        builder.Property(e => e.DataType)
            .HasMaxLength(100)
            .IsRequired();

        // DataValue stored as JSONB for flexibility
        builder.Property(e => e.DataValue)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.DataUom)
            .HasMaxLength(20)
            .IsRequired();

        // ConfidenceScore value object conversion
        builder.Property(e => e.ConfidenceScore)
            .HasConversion(
                v => v.Value,
                v => new ConfidenceScore(v))
            .HasColumnType("decimal(3,2)")
            .IsRequired();

        builder.Property(e => e.ExtractedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.Context);

        builder.Property(e => e.AgentConfigurationId)
            .IsRequired();

        // Optional JSONB column for processing metadata
        builder.Property(e => e.ProcessingMetadata)
            .HasColumnType("jsonb");

        // Configure table with constraints
        builder.ToTable("ExtractedData", t =>
        {
            t.HasCheckConstraint("CK_ExtractedData_ConfidenceScore", 
                "\"ConfidenceScore\" >= 0.0 AND \"ConfidenceScore\" <= 1.0");
        });

        // Indexes for performance
        builder.HasIndex(e => new { e.DataType, e.ExtractedAt })
            .HasDatabaseName("IX_ExtractedData_DataType_ExtractedAt");

        builder.HasIndex(e => e.AgentType)
            .HasDatabaseName("IX_ExtractedData_AgentType");

        builder.HasIndex(e => e.SourceFilePath)
            .HasDatabaseName("IX_ExtractedData_SourceFilePath");

        // JSONB index for DataValue queries
        builder.HasIndex(e => e.DataValue)
            .HasDatabaseName("IX_ExtractedData_DataValue")
            .HasMethod("gin");

        // Foreign key relationship
        builder.HasOne(e => e.AgentConfiguration)
            .WithMany(a => a.ExtractedData)
            .HasForeignKey(e => e.AgentConfigurationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events for EF Core
        builder.Ignore(e => e.DomainEvents);
    }
}