using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JobScraper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IScrapingService, ScrapingService>();

        return services;
    }
}