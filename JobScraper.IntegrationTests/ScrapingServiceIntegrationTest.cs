using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Application.Features.Scraping.Services;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.ClaudeApi;
using JobScraper.Infrastructure.Persistence.Repositories;
using JobScraper.Infrastructure.Scrapers;
using JobScraper.Infrastructure.Scraping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

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
                                { "SELENIUM_URL", SeleniumUrl }
                        })
                        .Build();


                // Setup all required dependencies
                var scrapingServiceLogger = Mock.Of<ILogger<ScrapingService>>();
                var websiteRepository = new WebsiteRepository(DbContext);
                var jobListingRepository = new JobListingRepository(DbContext);
                var cityRepository = new CityRepository(DbContext);
                var searchTermRepository = new SearchTermRepository(DbContext);
                var scraperLogger = Mock.Of<ILogger<JobnetScraper>>();
                var webScraperFactoryLogger = Mock.Of<ILogger<WebScraperFactory>>();
                var webdriverFactory = new WebDriverFactory(configuration, Mock.Of<ILogger<WebDriverFactory>>());
                var jobnetScraper = new JobnetScraper(webdriverFactory, scraperLogger);
                var webScraperFactory = new WebScraperFactory(webScraperFactoryLogger, jobnetScraper);
                var technologyTagRepository = new TechnologyTagRepository(DbContext);
                var claudeApiClient = new ClaudeApiClient(new HttpClient(), Mock.Of<ILogger<ClaudeApiClient>>());
                Console.WriteLine("Creating test website...");

                var website = Website.Create(
                        "https://job.jobnet.dk/CV/FindWork?",
                        "jobnet",
                        ["Systemudvikling, programmering og design"]).Value;

                await DbContext.Websites.AddAsync(website);
                await DbContext.SaveChangesAsync();

                Console.WriteLine("Starting scraping service...");
                var scrapingService = new ScrapingService(scrapingServiceLogger, websiteRepository, webScraperFactory,
                        jobListingRepository, cityRepository,
                        searchTermRepository, claudeApiClient, technologyTagRepository);

                await scrapingService.InitiateScrape(CancellationToken.None);

                Console.WriteLine("Checking database for saved jobs...");
                var savedJobs = await DbContext.JobListings.ToListAsync();
                Console.WriteLine($"Found {savedJobs.Count} saved jobs");

                Assert.IsTrue(savedJobs.Any(), "No jobs were saved to the database");
        }
}