using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobScraper.Infrastructure.Persistence.Configurations;

public class ScrapingErrorConfiguration : IEntityTypeConfiguration<ScrapingError>
{
    public void Configure(EntityTypeBuilder<ScrapingError> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(e => e.Timestamp);
        
        builder.Property(e => e.Message);
        
        builder.Property(e => e.StackTrace);

        builder.Property(e => e.RetryCount);
        
        builder.Property(e => e.ErrorType);

        builder.HasMany(e => e.Websites)
            .WithMany(e => e.ScrapingErrors);
    }
}
