using ErrorOr;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Mappers;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : BackgroundService, IScrapingService
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly IWebsiteRepository _websiteRepository;
    private readonly IJobListingRepository _jobListingRepository;
    private readonly IWebScraperFactory _webScraperFactory;
    private readonly IScrapeResultMapper _scrapeResultMapper;

    public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository,
        IWebScraperFactory webScraperFactory, IJobListingRepository jobListingRepository,
        IScrapeResultMapper scrapeResultMapper)
    {
        _logger = logger;
        _websiteRepository = websiteRepository;
        _webScraperFactory = webScraperFactory;
        _jobListingRepository = jobListingRepository;
        _scrapeResultMapper = scrapeResultMapper;
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
        if (!websites.Any())
        {
            _logger.LogError("No websites in database");
            return Error.NotFound("No websites found");
        }

        var scrapeResults = await ScrapeWebsitesAsync(websites, cancellationToken);
        var (successfulScrapes, failedScrapes) = SortScrapeResults(scrapeResults);
        if (successfulScrapes.Count == 0 && failedScrapes.Count == 0)
        {
            return Error.NotFound("No scraping results found");
        }

        var recentExistingListingsFromDb = _jobListingRepository.GetRecentListings(cancellationToken).ToList();
        var (newListings, existingScrapedListings) = SeparateNewAndExistingListings(successfulScrapes, recentExistingListingsFromDb);
        existingScrapedListings = AddNewSearchTermsToExistingListings(existingScrapedListings);

        // Map to entities
        var newJobListings = _scrapeResultMapper.MapToJobListings(newListings);


        return Result.Success;
    }

    private Dictionary<ScrapedJobData,JobListing> AddNewSearchTermsToExistingListings(
        Dictionary<ScrapedJobData,JobListing> existingScrapedListings)
    {
        foreach (var (scrapedListing, existingListing) in existingScrapedListings)
        {
            if (existingListing.SearchTerms.Select(s => s.Value).Contains(scrapedListing.SearchTerm)) continue;
            
            var searchTerm = _scrapeResultMapper.MapToSearchTerm(scrapedListing, existingListing);
            existingListing.SearchTerms.Add(searchTerm);
        }
        return existingScrapedListings;
    }


    internal static (List<ScrapedJobData> newListings, Dictionary<ScrapedJobData, JobListing> existingScrapedListings)
        SeparateNewAndExistingListings(List<ScrapedJobData?> successfulScrapes, List<JobListing> recentExistingListingsFromDb)
    {
        var newListings = new List<ScrapedJobData>();
        var existingMatches = new Dictionary<ScrapedJobData, JobListing>();

        foreach (var scrapedListing in successfulScrapes)
        {
            var scrapedListingCity = LocationParser.ExtractCityName(scrapedListing.Location);
            var matchingListing = recentExistingListingsFromDb.FirstOrDefault(
                existingListing => existingListing.Title == scrapedListing.Title &&
                                   existingListing.CompanyName == scrapedListing.CompanyName &&
                                   existingListing.City.Name == scrapedListingCity ||
                                   existingListing.Url == scrapedListing.Url);

            if (matchingListing != null)
                existingMatches.Add(scrapedListing, matchingListing);
            else
                newListings.Add(scrapedListing);
        }

        return (newListings, existingMatches);
        // if (successfulScrapes.Count == 0) return new List<ScrapedJobData?>();
        //
        // return successfulScrapes.Where(newListing =>
        //     !recentExistingListings.Any(existingListing =>
        //         (existingListing.Url == newListing?.Url ||
        //          (existingListing.CompanyName == newListing?.CompanyName &&
        //           existingListing.Title == newListing.Title &&
        //           existingListing.City.Name == LocationParser.ExtractCityName(newListing.Location)))));

        // var newListings = new List<ScrapedJobData>(); //
        // foreach (var scrapedListing in successfulScrapes)
        // {
        //     var scrapedListingCity = scrapedListing.Location.Split(' ').Last();
        //     var matchingListing = recentExistingListings.FirstOrDefault(
        //         existingListing => existingListing.Title == scrapedListing.Title &&
        //         existingListing.CompanyName == scrapedListing.CompanyName &&
        //         existingListing.City.Name == scrapedListingCity ||
        //         existingListing.Url == scrapedListing.Url);
        //     
        //     if (matchingListing != null) continue;
        //     
        //     newListings.Add(scrapedListing);
        // }
        // return newListings;
    }

    internal static (List<ScrapedJobData?> successfulScrapes, List<FailedJobScrape?> failedScrapes)
        SortScrapeResults(List<ScrapingResult> scrapeResults)
    {
        var successfulScrapes = scrapeResults.Select(r => r.ScrapedJobData)
            .Where(scrapedJobData => scrapedJobData != null).ToList();
        
        var failedScrapes = scrapeResults.Select(r => r.FailedJobScrape)
            .Where(failedScrape => failedScrape != null).ToList();

        return (successfulScrapes, failedScrapes);
    }

    private async Task<List<ScrapingResult>> ScrapeWebsitesAsync(IEnumerable<Website> websites,
        CancellationToken cancellationToken)
    {
        var scrapingResultsForAllWebsites = new List<ScrapingResult>();
        foreach (var website in websites)
        {
            _logger.LogInformation("Scraping website {website}", website.ShortName);
            var scraper = _webScraperFactory.TryCreateWebScraper(website.ShortName);
            var scrapeRequest = new ScrapeRequest(website.Url, website.SearchTerms.Select(s => s.Value).ToList());
            var result = await scraper.ScrapePageAsync(scrapeRequest, cancellationToken);
            scrapingResultsForAllWebsites.AddRange(result);
        }
        _logger.LogInformation("All websites scraped");

        return scrapingResultsForAllWebsites;
    }
}