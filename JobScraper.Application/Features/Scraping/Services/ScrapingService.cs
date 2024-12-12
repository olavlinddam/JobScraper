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
    private readonly ICityRepository _cityRepository;
    private readonly ISearchTermRepository _searchTermRepository;
    
    private readonly IWebScraperFactory _webScraperFactory;
    private readonly IScrapeResultMapper _scrapeResultMapper;
    

    public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository,
        IWebScraperFactory webScraperFactory, IJobListingRepository jobListingRepository,
        IScrapeResultMapper scrapeResultMapper, ICityRepository cityRepository, ISearchTermRepository searchTermRepository)
    {
        _logger = logger;
        _websiteRepository = websiteRepository;
        _webScraperFactory = webScraperFactory;
        _jobListingRepository = jobListingRepository;
        _scrapeResultMapper = scrapeResultMapper;
        _cityRepository = cityRepository;
        _searchTermRepository = searchTermRepository;
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

        await HandleNewCities(successfulScrapes, cancellationToken);
        var newJobListings = HandleScrapedListings(successfulScrapes, websites, cancellationToken);
        
        return Result.Success;
    }

    private async List<JobListing> HandleScrapedListings(List<ScrapedJobData?> successfulScrapes,
        List<Website> websites, CancellationToken cancellationToken)
    {
        // Fetch required entities from db
        var cities = await _cityRepository.GetAll(cancellationToken);
        var recentExistingListings = _jobListingRepository.GetRecentListings(cancellationToken).ToList();
        var searchTerms = _searchTermRepository.GetAllAsync(cancellationToken);
        
        // Find new listings
        var (scrapedListingsNotInDb, existingScrapedListings) = SeparateNewAndExistingListings(successfulScrapes, recentExistingListings);
        
        // Update existing listings with new search terms
        existingScrapedListings = UpdateExistingListingsSearchTerms(existingScrapedListings);
        
        // Map to new listings
        var newListings = _scrapeResultMapper.MapToJobListings(scrapedListingsNotInDb, cities, websites, searchTerms);
        
        // Update existing and save new entities to DB
        
        
        
        var newJobListings = _scrapeResultMapper.MapToJobListings(newListings);
    }

    private async Task HandleNewCities(List<ScrapedJobData?> successfulScrapes, CancellationToken cancellationToken)
    {
        if (successfulScrapes.Count == 0)
        {
            return;
        }

        var citiesFromScrape = successfulScrapes.Select(scrapedJob => _scrapeResultMapper.MapToCities(scrapedJob)).ToList();
        
        var existingCities = await _cityRepository.GetAll(cancellationToken);
        var newCities = ExtractNewCities(existingCities, citiesFromScrape);
        if (newCities.Count == 0)
        {
            return;
        }
        await _cityRepository.AddRangeAsync(newCities, cancellationToken);
    }

    private List<City> ExtractNewCities(List<City> existingCities, List<City> citiesFromScrape)
    {
        if (existingCities.Count == 0)
        {
            return citiesFromScrape;
        }
        
        var existingZipCodes = existingCities.Select(existingCity => existingCity.Zip);
        var newCities = citiesFromScrape.Where(scrapedCity => !existingZipCodes.Contains(scrapedCity.Zip)).ToList();
        return newCities;
    }


    private Dictionary<ScrapedJobData,JobListing> UpdateExistingListingsSearchTerms(
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