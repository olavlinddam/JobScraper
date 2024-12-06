using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Infrastructure.Tests.ScraperTests.JobnetScraper;

[TestClass]
public class ScrapePageTest
{
    [TestMethod]
    public async Task ScrapePage()
    {
        var scraper = new Scrapers.JobnetScraper();

        var website = new Website()
        {
            Url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch"
        };

        var scrapeRequest = new ScrapeRequest()
        {
            SearchTerm = "Systemudvikling, programmering og design"
        };
        
        await scraper.ScrapePageAsync(website, scrapeRequest, CancellationToken.None);

    }
}