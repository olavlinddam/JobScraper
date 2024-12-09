using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Infrastructure.Tests.ScraperTests.JobnetScraper;

[TestClass]
public class BuildUrlTests
{
    [TestMethod]
    public void BuildUrl_WithSearchTermOnly_ShouldReturnSingleParameterUrl()
    {
        // Arrange
        var url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch";
        var searchTerms = new List<string>() { "Systemudvikling, programmering og design" };
        var scrapeRequest = new ScrapeRequest(url, searchTerms);

        // Act
        var result = Scrapers.JobnetScraper.BuildUrls(scrapeRequest);

        // Assert
        var expected = new List<string>()
        {
            "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&SearchString=%2522Systemudvikling,%2520" +
            "programmering%2520og%2520design%2522"
        };

        Assert.AreEqual(expected[0], result[0]);
    }

    // [TestMethod]
    // public void BuildUrl_WithZipAndDistance_ShouldReturnSingleParameterUrl()
    // {
    //     // Arrange
    //     var website = new Website()
    //     {
    //         Url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch"
    //     };
    //
    //     var scrapeRequest = new ScrapeRequest()
    //     {
    //         Location = "5000",
    //         DistanceFromLocation = "50"
    //     };
    //
    //     // Act
    //     var result = Scrapers.JobnetScraper.BuildUrls(website, scrapeRequest);
    //
    //     // Assert
    //     Assert.AreEqual(
    //         "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&LocationZip=5000&SearchInGeoDistance=50",
    //         result);
    // }
}