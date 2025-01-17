using ErrorOr;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Mapping;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : IScrapingService
{
    private readonly ILogger<ScrapingService> _logger;
    private readonly IWebsiteRepository _websiteRepository;
    private readonly IJobListingRepository _jobListingRepository;
    private readonly ICityRepository _cityRepository;
    private readonly ISearchTermRepository _searchTermRepository;

    private readonly IWebScraperFactory _webScraperFactory;


    public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository,
        IWebScraperFactory webScraperFactory, IJobListingRepository jobListingRepository,
        ICityRepository cityRepository, ISearchTermRepository searchTermRepository)
    {
        _logger = logger;
        _websiteRepository = websiteRepository;
        _webScraperFactory = webScraperFactory;
        _jobListingRepository = jobListingRepository;
        _cityRepository = cityRepository;
        _searchTermRepository = searchTermRepository;
    }


    public async Task<ErrorOr<Success>> InitiateScrape(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting all scrapers");

            var websites = (await _websiteRepository.GetWithSearchTerms(cancellationToken)).ToList();
            if (websites.Count == 0)
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

            await HandleScrapedListings(successfulScrapes, websites, cancellationToken);
            await UpdateWebsites(websites, cancellationToken);

            return Result.Success;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while scraping websites: [{e}]", e);
            throw;
        }
    }

    private async Task UpdateWebsites(List<Website> websites, CancellationToken cancellationToken)
    {
        websites.ForEach(w => w.LastScraped = DateTime.UtcNow);
        await _websiteRepository.UpdateRangeAsync(websites, cancellationToken);
    }

    private async Task HandleScrapedListings(List<ScrapedJobData?> successfulScrapes,
        List<Website> websites, CancellationToken cancellationToken)
    {
        // Fetch required entities from db
        var cities = await _cityRepository.GetAll(cancellationToken);
        var recentExistingListings = await _jobListingRepository.GetRecentListingsWithWebsitesAndSearchTerms(cancellationToken);
        var searchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);

        var (scrapedListingsNotInDb, existingScrapedListingsDict) =
            SeparateNewAndExistingListings(successfulScrapes, recentExistingListings);

        var existingScrapedListings = UpdateExistingListingsSearchTerms(existingScrapedListingsDict);

        var newListings = ScrapeResultMapper.MapToJobListings(scrapedListingsNotInDb, cities, websites, searchTerms);

        if (existingScrapedListings.Count > 0)
        {
            _logger.LogInformation("Updated {count} existing listings", existingScrapedListingsDict.Count);
            await _jobListingRepository.UpdateRangeAsync(existingScrapedListings, cancellationToken);
        }

        if (newListings.Count > 0)
        {
            _logger.LogInformation("Found {count} existing listings", newListings.Count);
            await _jobListingRepository.AddRangeAsync(newListings, cancellationToken);
        }
    }

    private List<JobListing> UpdateExistingListingsSearchTerms(
        Dictionary<ScrapedJobData, JobListing> existingScrapedListings)
    {
        foreach (var (scrapedListing, existingListing) in existingScrapedListings)
        {
            if (existingListing.SearchTerms.Select(s => s.Value).Contains(scrapedListing.SearchTerm)) continue;

            var searchTermToBeAddedToExistingListing = existingListing.Website.SearchTerms.FirstOrDefault(
                st => st.Value == scrapedListing.SearchTerm);

            if (searchTermToBeAddedToExistingListing != null)
                existingListing.SearchTerms.Add(searchTermToBeAddedToExistingListing);
        }

        return existingScrapedListings.Select(l => l.Value).ToList();
    }


    internal static (List<ScrapedJobData> newListings, Dictionary<ScrapedJobData, JobListing> existingScrapedListings)
        SeparateNewAndExistingListings(List<ScrapedJobData?> successfulScrapes,
            List<JobListing> recentExistingListingsFromDb)
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
            _logger.LogInformation("Finished scraping website {website}", website.ShortName);
        }

        return scrapingResultsForAllWebsites;
    }
}