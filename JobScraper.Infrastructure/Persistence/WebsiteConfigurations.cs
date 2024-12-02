using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence;

public class WebsiteConfigurations : IEntityTypeConfiguration<Website>
{
    // This is the configurations for how EF core should map the website entity to the DB.
    // Doing it this way, we dont have to write all this config in the AppDbContext class and we can keep the model
    // free of any DB related implementations. See AppDbContext.
    
    public void Configure(EntityTypeBuilder<Website> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Url).IsRequired().HasMaxLength(2000);
        
        builder.Property(w => w.ShortName).IsRequired().HasMaxLength(100);

        builder.Property(w => w.LastScraped);

        builder.HasOne(x => x.ScrapingPolicy)
            .WithMany(sp => sp.Websites)
            .HasForeignKey(x => x.ScrapingPolicyId);
        
        builder.HasMany(w => w.JobsListings)
            .WithOne(j => j.Website)
            .HasForeignKey(j => j.WebsiteId);

        builder.HasMany(w => w.ScrapingErrors)
            .WithMany(sc => sc.Websites);
    }
}