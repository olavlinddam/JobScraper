using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Services;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Persistence.Repositories;
using JobScraper.Infrastructure.Scrapers;
using JobScraper.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace JobScraper.IntegrationTests;

[TestClass]
public class SceapingServiceIntegrationTest : IntegrationTestBase
{
    [TestMethod]
    public async Task ScrapeWebsite_ShouldSaveJobListings()
    {
        // Arrange
        await InitializeAsync();

        Console.WriteLine("Setting up test dependencies...");
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "SeleniumUrl", SeleniumUrl }
            })
            .Build();

        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddConsole()
                .SetMinimumLevel(LogLevel.Debug));

        // Setup all required dependencies
        // var websiteRepository = new WebsiteRepository(DbContext);
        // var jobListingRepository = new JobListingRepository(DbContext);
        // var cityRepository = new CityRepository(DbContext);
        // var searchTermRepository = new SearchTermRepository(DbContext);
        // var scraperLogger = loggerFactory.CreateLogger<JobnetScraper>();
        // var webScraperFactoryLogger = loggerFactory.CreateLogger<WebScraperFactory>();
        // var scrapingServiceLogger = loggerFactory.CreateLogger<ScrapingService>();
        // var jobnetScraper = new JobnetScraper(scraperLogger);
        // var webScraperFactory = new WebScraperFactory(webScraperFactoryLogger, jobnetScraper); 
        //
        // Console.WriteLine("Creating test website...");
        // var website = Website.Create(
        //     "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch",
        //     "jobnet",
        //     ["Systemudvikling, programmering og design"]).Value;
        //
        // await DbContext.Websites.AddAsync(website);
        // await DbContext.SaveChangesAsync();

        Console.WriteLine("Starting scraping service...");
        // var scrapingService = new ScrapingService(
        //     scrapingServiceLogger,
        //     websiteRepository,
        //     webScraperFactory,
        //     jobListingRepository,
        //     cityRepository,
        //     searchTermRepository);
        //
        // await scrapingService.InitiateScrape(CancellationToken.None);

        Console.WriteLine("Checking database for saved jobs...");
        var savedJobs = await DbContext.JobListings.ToListAsync();
        Console.WriteLine($"Found {savedJobs.Count} saved jobs");

        Assert.IsTrue(savedJobs.Any(), "No jobs were saved to the database");
    }
}