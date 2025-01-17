using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Infrastructure.Persistence;
using JobScraper.Infrastructure.Persistence.Repositories;
using JobScraper.Infrastructure.Scrapers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScraper.Infrastructure.StartUp;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("LocalDb")));

        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var username = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var connectionString = $"Host={host};Database={database};Username={username};Password={password}";

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.TryAddScoped<IWebsiteRepository, WebsiteRepository>();
        services.TryAddScoped<IJobListingRepository, JobListingRepository>();
        services.TryAddScoped<ICityRepository, CityRepository>();
        services.TryAddScoped<ISearchTermRepository, SearchTermRepository>();


        // Scrapers
        services.TryAddScoped<IJobnetScraper, JobnetScraper>();

        return services;
    }
}