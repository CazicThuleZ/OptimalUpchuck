#nullable enable

using Microsoft.EntityFrameworkCore;
using OptimalUpchuck.Domain.Entities;
using OptimalUpchuck.Infrastructure.Data.Configurations;

namespace OptimalUpchuck.Infrastructure.Data;

/// <summary>
/// Application database context for Optimal Upchuck
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ElevationProposal> ElevationProposals { get; set; } = null!;
    public DbSet<ExtractedData> ExtractedData { get; set; } = null!;
    public DbSet<AgentConfiguration> AgentConfigurations { get; set; } = null!;
    public DbSet<ProcessingQueue> ProcessingQueue { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            throw new InvalidOperationException("DbContext is not configured. Ensure connection string is provided.");
        }

        // Enable sensitive data logging only in development
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new ElevationProposalConfiguration());
        modelBuilder.ApplyConfiguration(new ExtractedDataConfiguration());
        modelBuilder.ApplyConfiguration(new AgentConfigurationConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessingQueueConfiguration());

        // Enable uuid-ossp extension
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // Create database views
        CreateDatabaseViews(modelBuilder);

        // Database functions and triggers will be added via raw SQL in migrations
    }

    private static void CreateDatabaseViews(ModelBuilder modelBuilder)
    {
        // PendingProposalsSummary view
        modelBuilder.Entity<PendingProposalSummary>()
            .HasNoKey()
            .ToView("PendingProposalsSummary")
            .HasAnnotation("Relational:ViewDefinition", @"
                SELECT 
                    ep.""AgentType"",
                    COUNT(*) as ""PendingCount"",
                    AVG(ep.""ConfidenceScore"") as ""AverageConfidence"",
                    MIN(ep.""CreatedAt"") as ""OldestProposal"",
                    MAX(ep.""CreatedAt"") as ""NewestProposal""
                FROM ""ElevationProposals"" ep
                WHERE ep.""ReviewStatus"" = 'Pending'
                GROUP BY ep.""AgentType""");

        // AgentProcessingStats view
        modelBuilder.Entity<AgentProcessingStats>()
            .HasNoKey()
            .ToView("AgentProcessingStats")
            .HasAnnotation("Relational:ViewDefinition", @"
                SELECT 
                    ac.""AgentType"",
                    ac.""IsEnabled"",
                    ac.""AutonomyLevel"",
                    COUNT(DISTINCT ep.""Id"") as ""TotalProposals"",
                    COUNT(DISTINCT ed.""Id"") as ""TotalExtractions"",
                    COUNT(DISTINCT CASE WHEN ep.""ReviewStatus"" = 'Approved' THEN ep.""Id"" END) as ""ApprovedProposals"",
                    AVG(ep.""ConfidenceScore"") as ""AverageProposalConfidence"",
                    AVG(ed.""ConfidenceScore"") as ""AverageExtractionConfidence""
                FROM ""AgentConfigurations"" ac
                LEFT JOIN ""ElevationProposals"" ep ON ac.""Id"" = ep.""AgentConfigurationId""
                LEFT JOIN ""ExtractedData"" ed ON ac.""Id"" = ed.""AgentConfigurationId""
                GROUP BY ac.""Id"", ac.""AgentType"", ac.""IsEnabled"", ac.""AutonomyLevel""");
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Clear domain events after saving
        var proposalEntities = ChangeTracker.Entries<ElevationProposal>()
            .Select(e => e.Entity);
        var extractedDataEntities = ChangeTracker.Entries<ExtractedData>()
            .Select(e => e.Entity);

        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        foreach (var entity in proposalEntities)
        {
            entity.ClearDomainEvents();
        }
        
        foreach (var entity in extractedDataEntities)
        {
            entity.ClearDomainEvents();
        }

        return result;
    }
}

/// <summary>
/// View model for pending proposals summary
/// </summary>
public class PendingProposalSummary
{
    public string AgentType { get; set; } = string.Empty;
    public int PendingCount { get; set; }
    public decimal AverageConfidence { get; set; }
    public DateTime OldestProposal { get; set; }
    public DateTime NewestProposal { get; set; }
}

/// <summary>
/// View model for agent processing statistics
/// </summary>
public class AgentProcessingStats
{
    public string AgentType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string AutonomyLevel { get; set; } = string.Empty;
    public int TotalProposals { get; set; }
    public int TotalExtractions { get; set; }
    public int ApprovedProposals { get; set; }
    public decimal AverageProposalConfidence { get; set; }
    public decimal AverageExtractionConfidence { get; set; }
}