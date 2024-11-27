using JobScraper.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JobScraper.Infrastructure.Common.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Website> Websites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This is where all implementations of IEntityTypeBuilder<T> is applied. That is the configuration of 
        // the relationships and how entities are mapped to the db. See JobScraper.Infrastructure.Websites.Persistence.WebsiteConfiguration
        // for an example.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<AppDbContextFactory>()
            .Build();

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("LocalDb"));

        return new AppDbContext(optionsBuilder.Options);
    }
}