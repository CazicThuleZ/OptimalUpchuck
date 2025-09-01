#nullable enable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OptimalUpchuck.Domain.Entities;
using OptimalUpchuck.Domain.ValueObjects;

namespace OptimalUpchuck.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AgentConfiguration entity
/// </summary>
public class AgentConfigurationConfiguration : IEntityTypeConfiguration<AgentConfiguration>
{
    public void Configure(EntityTypeBuilder<AgentConfiguration> builder)
    {

        // Primary Key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        // Properties with constraints
        builder.Property(e => e.AgentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.IsEnabled)
            .HasDefaultValue(true)
            .IsRequired();

        // AutonomyLevel enum conversion
        builder.Property(e => e.AutonomyLevel)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // ConfidenceThreshold value object conversion
        builder.Property(e => e.ConfidenceThreshold)
            .HasConversion(
                v => v.Value,
                v => new ConfidenceScore(v))
            .HasColumnType("decimal(3,2)")
            .IsRequired();

        // JSONB columns
        builder.Property(e => e.ConfigurationJson)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(e => e.ModelParameters)
            .HasColumnType("jsonb");

        builder.Property(e => e.ProcessingRules)
            .HasColumnType("jsonb");

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'")
            .IsRequired();

        builder.Property(e => e.Version)
            .HasDefaultValue(1)
            .IsRequired();

        // Unique constraint on AgentType
        builder.HasIndex(e => e.AgentType)
            .IsUnique()
            .HasDatabaseName("IX_AgentConfigurations_AgentType_Unique");

        // Configure table with constraints
        builder.ToTable("AgentConfigurations", t =>
        {
            t.HasCheckConstraint("CK_AgentConfigurations_ConfidenceThreshold", 
                "\"ConfidenceThreshold\" >= 0.0 AND \"ConfidenceThreshold\" <= 1.0");

            t.HasCheckConstraint("CK_AgentConfigurations_AutonomyLevel",
                "\"AutonomyLevel\" IN ('ReviewRequired', 'SemiAutonomous', 'FullyAutonomous')");

            t.HasCheckConstraint("CK_AgentConfigurations_Version",
                "\"Version\" >= 1");
        });

        // Index for queries
        builder.HasIndex(e => e.IsEnabled)
            .HasDatabaseName("IX_AgentConfigurations_IsEnabled");

        // Relationships are already configured via navigation properties in the entity
        // The Many side will be configured in ElevationProposal and ExtractedData configurations
    }
}