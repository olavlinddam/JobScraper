using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence;

public class ScrapingPolicyConfiguration : IEntityTypeConfiguration<ScrapingPolicy>
{
    public void Configure(EntityTypeBuilder<ScrapingPolicy> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.RequestsPerMinute);
        
        builder.Property(p => p.ShouldRespectRobotsTxt);
        
        builder.Property(p => p.CooldownPeriod);

        builder.HasMany(p => p.Websites)
            .WithOne(p => p.ScrapingPolicy)
            .HasForeignKey(p => p.ScrapingPolicyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
