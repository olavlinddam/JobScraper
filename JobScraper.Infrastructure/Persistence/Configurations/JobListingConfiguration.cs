using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class JobListingConfiguration : IEntityTypeConfiguration<JobListing>
{
    public void Configure(EntityTypeBuilder<JobListing> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(l => l.CompanyName);

        builder.Property(l => l.PostedDate);

        builder.Property(l => l.ExpirationDate);

        builder.Property(l => l.Url);

        builder.Property(l => l.Description);


        builder.Property(l => l.ScrapedDate);

        builder.Property(l => l.JobType);

        builder.HasOne(l => l.City)
            .WithMany(c => c.JobListings)
            .HasForeignKey(l => l.CityId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(l => l.Website)
            .WithMany(w => w.JobsListings)
            .HasForeignKey(l => l.WebsiteId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}