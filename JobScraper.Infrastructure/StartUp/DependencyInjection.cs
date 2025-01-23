using System.Runtime.CompilerServices;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Infrastructure.ClaudeApi;
using JobScraper.Infrastructure.Persistence;
using JobScraper.Infrastructure.Persistence.Configurations;
using JobScraper.Infrastructure.Persistence.Repositories;
using JobScraper.Infrastructure.Scrapers;
using JobScraper.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScraper.Infrastructure.StartUp;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddPersistence(services);

        // Claude
        services.TryAddScoped<IClaudeApiClient, ClaudeApiClient>();

        // Scrapers
        services.TryAddScoped<WebDriverFactory>();
        services.TryAddScoped<IJobnetScraper, JobnetScraper>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        // Create DatabaseSettings from environment variables
        var dbSettings = new DatabaseSettings
        {
            Host = Environment.GetEnvironmentVariable("DB_HOST"),
            Name = Environment.GetEnvironmentVariable("DB_NAME"),
            User = Environment.GetEnvironmentVariable("DB_USER"),
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
        };

        // Validate required settings
        if (string.IsNullOrEmpty(dbSettings.Host) || string.IsNullOrEmpty(dbSettings.Name) ||
            string.IsNullOrEmpty(dbSettings.User) || string.IsNullOrEmpty(dbSettings.Password))
        {
            throw new InvalidOperationException("Database environment variables are missing or invalid.");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbSettings.ConnectionString));
        services.TryAddScoped<IWebsiteRepository, WebsiteRepository>();
        services.TryAddScoped<IJobListingRepository, JobListingRepository>();
        services.TryAddScoped<ICityRepository, CityRepository>();
        services.TryAddScoped<ISearchTermRepository, SearchTermRepository>();
        services.TryAddScoped<ITechnologyTagRepository, TechnologyTagRepository>();
        return services;
    }
}