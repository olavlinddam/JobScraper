using ErrorOr;
using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.Scraping.Common;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : IScrapingService
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly IWebsiteRepository _websiteRepository;
    private readonly ScraperStateManager _scraperStateManager;

    public ScrapingService(IWebsiteRepository websiteRepository, ScraperStateManager scraperStateManager, ILogger<ScrapingService> logger)
    {
        _websiteRepository = websiteRepository;
        _scraperStateManager = scraperStateManager;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> StartAllScrapers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting all scrapers");   
        
        // Get all relevant data form database
        var websites = await _websiteRepository.GetAllWithPolicyAsync(cancellationToken);

        if (!websites.Any())
        {
            _logger.LogError("No websites in database");
            return Error.NotFound("No websites found");
        }

        foreach (var website in websites)
        {
            if (_scraperStateManager.TryStartScraping(website.Id))
            {
                _logger.LogInformation("Starting scraper with id {Id}", website.Id);
                
                
                
                _scraperStateManager.StopScraping(website.Id);
            }
        }

        return Result.Success;
    }
}