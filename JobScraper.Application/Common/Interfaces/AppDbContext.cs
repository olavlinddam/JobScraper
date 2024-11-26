using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Website> Websites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Here we are dynamically applying all entity configuration classes from the AppDbContext assembly. 
        // ApplyConfigurationsFromAssembly scans for classes implementing IEntityTypeConfiguration<T> for any entity.
        // All found configurations are applied to the model when creating it.
        // See JobScraper.Infrastructure.Websites.Persistence.WebsiteConfiguration for an example.
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
                    base.OnModelCreating(modelBuilder);
    }
}