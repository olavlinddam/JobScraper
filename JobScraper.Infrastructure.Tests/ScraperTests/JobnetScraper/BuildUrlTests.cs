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
        var website = new Website()
        {
            Url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch"
        };

        var scrapeRequest = new ScrapeRequest()
        {
            SearchTerm = "Systemudvikling, programmering og design"
        };
        
        // Act
        var result = Scrapers.JobnetScraper.BuildUrls(website, scrapeRequest);
        
        // Assert
        Assert.AreEqual("https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&SearchString=%2522Systemudvikling,%2520programmering%2520og%2520design%2522", result);
    } 
    
    [TestMethod]
    public void BuildUrl_WithZipAndDistance_ShouldReturnSingleParameterUrl()
    {
        // Arrange
        var website = new Website()
        {
            Url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch"
        };

        var scrapeRequest = new ScrapeRequest()
        {
            Location = "5000",
            DistanceFromLocation = "50"
        };
        
        // Act
        var result = Scrapers.JobnetScraper.BuildUrls(website, scrapeRequest);
        
        // Assert
        Assert.AreEqual("https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&LocationZip=5000&SearchInGeoDistance=50", result);
    } 
}