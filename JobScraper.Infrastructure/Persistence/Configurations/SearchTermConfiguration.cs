using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class SearchTermConfiguration : IEntityTypeConfiguration<SearchTerm>
{
    public void Configure(EntityTypeBuilder<SearchTerm> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.HasIndex(s => s.Value)
            .IsUnique();

        builder.Property(s => s.Value)
            .HasMaxLength(100);

        builder.Property(s => s.MatchingJobsCount)
            .HasDefaultValue(0);

        builder.Property(s => s.LastUsed);

        builder.HasMany(s => s.Websites)
            .WithMany(w => w.SearchTerms);

        builder.HasMany(s => s.JobListings)
            .WithMany(j => j.SearchTerms);
    }
}