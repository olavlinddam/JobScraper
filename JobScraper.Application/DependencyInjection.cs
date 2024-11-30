using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using JobScraper.Application.Features.Scraping.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScraper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton<ScraperStateManager>();

        return services;
    }
}