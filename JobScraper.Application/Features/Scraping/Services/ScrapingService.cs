using ErrorOr;
using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.Scraping.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : BackgroundService, IScrapingService, IDisposable
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly IWebsiteRepository _websiteRepository;

    public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository)
    {
        _logger = logger;
        _websiteRepository = websiteRepository;
    }

    public async Task<ErrorOr<Success>> ScrapeAllWebsites(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting all scrapers");

        // Get all relevant data form database
        var websites = await _websiteRepository.GetAllWithPoliciesAsync(cancellationToken);

        if (!websites.Any())
        {
            _logger.LogError("No websites in database");
            return Error.NotFound("No websites found");
        }

        foreach (var website in websites)
        {
            _logger.LogInformation("Starting scraper with id {Id}", website.Id);
            
            // resolve scraper type
            // start each scraper in parallel
            // store the scraped jobs in the database
            // store the errors in the database
        }

        return Result.Success;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Scraping jobs started");

                await ScrapeAllWebsites(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(120), cancellationToken); // TODO: NOT HARDCODE THE TIMER
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Scraping jobs cancelled");
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while scraping for jobs: {e}", e);
        }
    }
}