using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobScraper.Infrastructure.Tests.ScraperTests.JobnetScraper;

[TestClass]
public class ScrapePageTest
{
    [TestMethod]
    public async Task ScrapePage()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<Scrapers.JobnetScraper>>();
        var scraper = new Scrapers.JobnetScraper(loggerMock.Object); // Real scraper, mock logger

        var website = new Website()
        {
            Url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch"
        };
        var scrapeRequest = new ScrapeRequest();

        // Act
        await scraper.ScrapePageAsync(website, scrapeRequest, CancellationToken.None);

        // Assert
        // Add assertions about what was scraped
    }
}