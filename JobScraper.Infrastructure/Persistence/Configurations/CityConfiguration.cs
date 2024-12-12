using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name);
        
        builder.Property(c => c.Country);

        builder.Property(c => c.Zip);
        
        builder.HasMany(c => c.JobListings)
            .WithOne(c => c.City)
            .HasForeignKey(c => c.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}