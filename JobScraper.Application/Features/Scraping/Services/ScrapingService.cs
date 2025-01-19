using ErrorOr;
using JobScraper.Application.Common.ExceptionExtension;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Dtos;
using JobScraper.Application.Features.Scraping.Mapping;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Logging;
using ReverseMarkdown;

namespace JobScraper.Application.Features.Scraping.Services;

public class ScrapingService : IScrapingService
{
        private readonly ILogger<ScrapingService> _logger;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IJobListingRepository _jobListingRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ISearchTermRepository _searchTermRepository;
        private readonly ITechnologyTagRepository _technologyTagRepository;
        private readonly IClaudeApiClient _claudeApiClient;

        private readonly IWebScraperFactory _webScraperFactory;


        public ScrapingService(ILogger<ScrapingService> logger, IWebsiteRepository websiteRepository,
                IWebScraperFactory webScraperFactory, IJobListingRepository jobListingRepository,
                ICityRepository cityRepository, ISearchTermRepository searchTermRepository, IClaudeApiClient claudeApiClient,
                ITechnologyTagRepository technologyTagRepository)
        {
                _logger = logger;
                _websiteRepository = websiteRepository;
                _webScraperFactory = webScraperFactory;
                _jobListingRepository = jobListingRepository;
                _cityRepository = cityRepository;
                _searchTermRepository = searchTermRepository;
                _claudeApiClient = claudeApiClient;
                _technologyTagRepository = technologyTagRepository;
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

                        List<ScrapingResult> scrapingResults = [];
                        foreach (var website in websites)
                        {
                                var scrapingResult = await ScrapeWebsitesAsync(website, cancellationToken);
                                scrapingResults.AddRange(scrapingResult);
                        }

                        var (successfulScrapes, failedScrapes) = SortScrapeResults(scrapingResults);
                        if (successfulScrapes.Count == 0 && failedScrapes.Count == 0)
                        {
                                return Error.NotFound("No scraping results found");
                        }

                        await HandleSuccessfulScrapes(successfulScrapes, websites, cancellationToken);
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

        private async Task HandleSuccessfulScrapes(List<SuccessFullScrape?> successfulScrapes,
                List<Website> websites, CancellationToken cancellationToken)
        {
                // Fetch required entities from db
                var cities = await _cityRepository.GetAll(cancellationToken);
                var recentExistingListings =
                        await _jobListingRepository.GetRecentListingsWithWebsitesAndSearchTerms(cancellationToken);
                var searchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);
                var technologyTags = await _technologyTagRepository.GetAllAsync(cancellationToken);


                var (scrapedListingsNotInDb, existingScrapedListingsDict) =
                        SeparateNewAndExistingListings(successfulScrapes, recentExistingListings);

                var existingScrapedListings = UpdateExistingListingsSearchTerms(existingScrapedListingsDict);

                var scrapedNewListingsWithClaudeResponseResult = await AddClaudeResponse(scrapedListingsNotInDb, technologyTags);

                var newListings = ScrapeResultMapper.MapToJobListings(scrapedNewListingsWithClaudeResponseResult, cities,
                        websites, searchTerms);

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

        private async Task<List<SuccessFullScrape>> AddClaudeResponse(List<SuccessFullScrape> scrapedListingsNotInDb,
                List<TechnologyTag> technologyTags)
        {
                try
                {
                        var analysisRequests = CreateAnalysisRequests(scrapedListingsNotInDb, technologyTags);
                        var claudeResponses = await _claudeApiClient.AnalyzeJobListingsBatch(analysisRequests);
                        scrapedListingsNotInDb = UpdateListingsWithResponses(scrapedListingsNotInDb, claudeResponses);
                        return scrapedListingsNotInDb;
                }
                catch (ClaudeApiIntegrationException e)
                {
                        _logger.LogError("Claude API integration failed while processing job listings: [{message}].", e.Message);
                        throw;
                }
                catch (UpdateScrapedJobException e)
                {
                        _logger.LogError(
                                "Scraping Service failed while updating scraped jobs with Claude API responses: [{message}].",
                                e.Message);
                        throw;
                }
        }

        internal static List<SuccessFullScrape> UpdateListingsWithResponses(List<SuccessFullScrape> scrapedListingsNotInDb,
                List<ClaudeApiJobListingAnalysis> claudeResponses)
        {
                try
                {
                        for (var i = 0; i < scrapedListingsNotInDb.Count; i++)
                        {
                                var listing = scrapedListingsNotInDb[i];
                                var claudeResponse = claudeResponses.FirstOrDefault(r => r.Index == i);
                                if (claudeResponse == null)
                                        throw new UpdateScrapedJobException(
                                                $"Could not find matching claude response to listing: [{listing.Url}]");

                                if (claudeResponse.IsSuccess == false)
                                {
                                        listing.LanguageCode = "Invalid";
                                        listing.Tags = [];
                                        listing.YearsOfExperience = -1;
                                }

                                listing.YearsOfExperience = claudeResponse.ApiAnalysisResponse.YearsOfExperience;
                                listing.Tags = claudeResponse.ApiAnalysisResponse.tags;
                                listing.LanguageCode = claudeResponse.ApiAnalysisResponse.LanguageCode;
                                listing.Description = claudeResponse.ApiAnalysisResponse.Summary;
                        }

                        return scrapedListingsNotInDb;
                }
                catch (Exception e)
                {
                        throw new UpdateScrapedJobException(e.Message, e.InnerException);
                }
        }

        private List<ClaudeApiAnalysisRequest> CreateAnalysisRequests(List<SuccessFullScrape> scrapedListingsNotInDb,
                List<TechnologyTag> technologyTags)
        {
                var converter = new Converter();
                return scrapedListingsNotInDb.Select((l, index) => new ClaudeApiAnalysisRequest
                {
                        ArticleMarkdown = converter.Convert(l.ArticleHtml),
                        Index = index,
                        Tags = technologyTags.Select(t => t.Name).ToList()
                }).ToList();
        }

        private List<JobListing> UpdateExistingListingsSearchTerms(
                Dictionary<SuccessFullScrape, JobListing> existingScrapedListings)
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


        internal static (List<SuccessFullScrape> newListings, Dictionary<SuccessFullScrape, JobListing> existingScrapedListings)
                SeparateNewAndExistingListings(List<SuccessFullScrape?> successfulScrapes,
                        List<JobListing> recentExistingListingsFromDb)
        {
                var newListings = new List<SuccessFullScrape>();
                var existingMatches = new Dictionary<SuccessFullScrape, JobListing>();

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

        internal static (List<SuccessFullScrape?> successfulScrapes, List<FailedJobScrape?> failedScrapes)
                SortScrapeResults(List<ScrapingResult> scrapeResults)
        {
                var successfulScrapes = scrapeResults.Select(r => r.SuccessFullScrape)
                        .Where(scrapedJobData => scrapedJobData != null).ToList();

                var failedScrapes = scrapeResults.Select(r => r.FailedJobScrape)
                        .Where(failedScrape => failedScrape != null).ToList();

                return (successfulScrapes, failedScrapes);
        }

        private async Task<List<ScrapingResult>> ScrapeWebsitesAsync(Website website,
                CancellationToken cancellationToken)
        {
                var scrapingResultsForAllWebsites = new List<ScrapingResult>();
                _logger.LogInformation("Scraping website {website}", website.ShortName);
                foreach (var searchTerm in website.SearchTerms)
                {
                        var result = await ScrapeSearchTerm(website, searchTerm, cancellationToken);
                        scrapingResultsForAllWebsites.AddRange(result);
                }

                _logger.LogInformation("Finished scraping website {website}", website.ShortName);
                return scrapingResultsForAllWebsites;
        }

        private async Task<IEnumerable<ScrapingResult>> ScrapeSearchTerm(Website website, SearchTerm searchTerm,
                CancellationToken cancellationToken)
        {
                var scraper = _webScraperFactory.TryCreateWebScraper(website.ShortName);
                var scrapeRequest = BuildScrapeRequest(website, searchTerm);
                var result = await scraper.ScrapeAsync(scrapeRequest, cancellationToken);
                return result;
        }

        private ScrapeRequest BuildScrapeRequest(Website website, SearchTerm searchTerm)
        {
                var latestListing = website.JobsListings
                        .Where(l => l.SearchTerms.Select(s => s.Value).Contains(searchTerm.Value))
                        .OrderByDescending(l => l.PostedDate).FirstOrDefault();
                return new ScrapeRequest(website.Url, searchTerm.Value,
                        "https://job.jobnet.dk/CV/FindWork/Details/114b7b1a-e508-498a-a938-8eddf13d8890");
        }
}