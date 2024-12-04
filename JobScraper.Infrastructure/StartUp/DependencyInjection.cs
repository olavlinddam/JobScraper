using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Infrastructure.Builders;
using JobScraper.Infrastructure.Persistence;
using JobScraper.Infrastructure.Persistence.Websites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScraper.Infrastructure.StartUp;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Getting the connection string from the dotnet user-secrets management
        // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-9.0&tabs=linux
        
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("LocalDb")));
        
        services.TryAddScoped<IWebScraperFactory, WebScraperFactory>();
        
        services.TryAddScoped<IWebsiteRepository, WebsiteRepository>();
        
        return services;
    }
}