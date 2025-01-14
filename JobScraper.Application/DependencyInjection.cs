using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.CityManagement;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Application.Features.JobListings;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Services;
using JobScraper.Application.Features.WebsiteManagement.Services;
using JobScraper.Application.Features.WebsiteManagement.Validation;
using JobScraper.Contracts.Requests.Websites;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace JobScraper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddScoped<IScrapingService, ScrapingService>();
        services.TryAddScoped<IWebsiteManagementService, WebsiteManagementService>();
        services.TryAddScoped<JobListingService>();
        services.TryAddScoped<CityService>();

        services.TryAddScoped<IWebScraperFactory, WebScraperFactory>();

        services.AddValidatorsFromAssemblyContaining<AddWebsiteRequestValidator>();
        return services;
    }
}