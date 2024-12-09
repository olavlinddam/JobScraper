using ErrorOr;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Scrapers;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : BackgroundService, IScrapingService, IDisposable
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly IWebsiteRepository _websiteRepository;
    private readonly IWebScraperFactory _webScraperFactory;

    public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository,
        IWebScraperFactory webScraperFactory)
    {
        _logger = logger;
        _websiteRepository = websiteRepository;
        _webScraperFactory = webScraperFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("ScrapingService initialized");

                var scrapingResults = await InitiateScrape(cancellationToken);


                await Task.Delay(TimeSpan.FromMinutes(120), cancellationToken); // TODO: NOT HARDCODE THE TIMER
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Scraping jobs cancelled");
        }
        catch (KeyNotFoundException e)
        {
            _logger.LogError("Initializing web scraper failed due to {e}:", e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while scraping for jobs: {e}", e);
        }
    }

    public async Task<ErrorOr<Success>> InitiateScrape(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting all scrapers");
        
        var websites = (await _websiteRepository.GetAllWithPoliciesAndSearchTermsAsync(cancellationToken)).ToList();
        if (websites.Count == 0)
        {
            _logger.LogError("No websites in database");
            return Error.NotFound("No websites found");
        }

        var scrapeResults = await ScrapeWebsitesAsync(websites, cancellationToken);
        var (successfulScrapes, failedScrapes) = SortScrapeResults(scrapeResults);
        if (successfulScrapes == null && failedScrapes == null)
        {
            return Error.NotFound("No scraping results found");
        }
        
        
        
        // Map to entities
        // Filter duplicates

        return Result.Success;
    }

    internal static (IEnumerable<ScrapedJobData?> successfulScrapes, IEnumerable<FailedJobScrape?> failedScrapes)
        SortScrapeResults(List<ScrapingResult>[] scrapeResults)
    {
        var successfulScrapes = scrapeResults.SelectMany(r => r)
            .Where(r => r.ScrapedJob != null)
            .Select(r => r.ScrapedJob);
        
        var failedScrapes = scrapeResults.SelectMany(r => r)
            .Where(r => r.FailedJobScrape != null)
            .Select(r => r.FailedJobScrape);
        
        return (successfulScrapes, failedScrapes);
    }

    private async Task<List<ScrapingResult>[]> ScrapeWebsitesAsync(List<Website> websites, CancellationToken cancellationToken)
    {
        // Running the scrapes in parallel to scrape all sites faster
        var scrapingTasks = websites.Select(async website =>
        {
            _logger.LogInformation("Scraping website {website}", website.ShortName);
            var scraper = _webScraperFactory.TryCreateWebScraper(website.ShortName);
            var scrapeRequest = new ScrapeRequest(website.Url, website.SearchTerms.Select(s => s.Value).ToList());
            return await scraper.ScrapePageAsync(scrapeRequest, cancellationToken);
        });

        var allResults = await Task.WhenAll(scrapingTasks);
        _logger.LogInformation("All websites scraped");

        return allResults;
    }
}
















