using JobScraper.Contracts.Requests.Scraping;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobScraper.Infrastructure.UnitTests.ScraperTests.JobnetScraper;

// [TestClass]
// public class ScrapePageTest
// {
//     [TestMethod]
//     public async Task ScrapePage()
//     {
//         // Arrange
//         var loggerMock = new Mock<ILogger<Scrapers.JobnetScraper>>();
//         var scraper = new Scrapers.JobnetScraper(loggerMock.Object);
//
//         var url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch";
//         var searchTerms = new List<string> { "Systemudvikling, programmering og design" };
//         var scrapeRequest = new ScrapeRequest(url, searchTerms);
//
//         // Act
//         await scraper.ScrapePageAsync(scrapeRequest, CancellationToken.None);
//
//         // Assert
//         // Add assertions about what was scraped
//     }
// }