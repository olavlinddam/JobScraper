using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobScraper.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<City> Cities { get; set; }
    public DbSet<JobListing> JobListings { get; set; }
    public DbSet<ScrapingError> ScrapingErrors { get; set; }
    public DbSet<SearchTerm> SearchTerms { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<TechnologyTag> TechnologyTags { get; set; }
    

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
        // Debug: Print environment variables
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var username = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        
        Console.WriteLine($"DB_HOST: {host ?? "null"}");
        Console.WriteLine($"DB_NAME: {database ?? "null"}");
        Console.WriteLine($"DB_USER: {username ?? "null"}");
        // Don't print the actual password, just whether it's null
        Console.WriteLine($"DB_PASSWORD is null: {password == null}");
        
        var connectionString = $"Host={host};Database={database};Username={username};Password={password}";
        Console.WriteLine($"Connection string: {connectionString}");
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
