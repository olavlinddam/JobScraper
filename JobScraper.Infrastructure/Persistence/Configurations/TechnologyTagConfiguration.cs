using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class TechnologyTagConfiguration : IEntityTypeConfiguration<TechnologyTag>
{
    public void Configure(EntityTypeBuilder<TechnologyTag> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name).IsRequired();

        builder.HasMany(t => t.JobListings)
            .WithMany(l => l.TechnologyTags);
    }
}