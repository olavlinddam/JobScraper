using ErrorOr;
using JobScraper.Application.Common.Interfaces;
using JobScraper.Application.Features.Scraping.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : IScrapingService
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly ScraperStateManager _scraperStateManager;
    private readonly IAppDbContext _dbContext;

    public ScrapingService(ScraperStateManager scraperStateManager, ILogger<ScrapingService> logger, IAppDbContext dbContext)
    {
        _scraperStateManager = scraperStateManager;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> StartAllScrapers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting all scrapers");   
        
        // Get all relevant data form database
        var websites = await _dbContext.Websites
            .Include(w => w.ScrapingPolicy).ToListAsync(cancellationToken);

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