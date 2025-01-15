using JobScraper.Application.Features.Scraping.Dtos;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.ClaudeApi;
using JobScraper.Infrastructure.Scrapers;
using JobScraper.Infrastructure.Scraping;
using JobScraper.IntegrationTests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Frameworks;

namespace JobScraper.IntegrationTests.Scrapers;

[TestClass]
public class JobnetScraperIntegrationTests : IntegrationTestBase
{
        [TestMethod]
        public async Task ScrapeJobnet_ShouldSucceed()
        {
                // Arrange
                await InitializeAsync();

                var configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                                { "SELENIUM_URL", SeleniumUrl }
                        }!)
                        .Build();

                var jobnetScraperLoggerMock = Mock.Of<ILogger<JobnetScraper>>();
                var webDriverFactoryLoggerMock = Mock.Of<ILogger<WebDriverFactory>>();
                var scraper = new JobnetScraper(new WebDriverFactory(
                        configuration, webDriverFactoryLoggerMock), jobnetScraperLoggerMock);
                var scrapingRequest = new ScrapeRequest("https://job.jobnet.dk/CV/FindWork?",
                        "Systemudvikling, programmering og design",
                        "https://zebon.dk/om-zebon/job/udvikler/"); // specific url taken from the website, not the best idea...
                

                CancellationToken ct = default;

                // act
                var results = await scraper.ScrapeAsync(scrapingRequest, ct);

                // Assert
                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count != 0);
                Assert.IsTrue(results.Select(r => r.SuccessFullScrape.Url).Contains("https://zebon.dk/om-zebon/job/udvikler/"));
        }
}