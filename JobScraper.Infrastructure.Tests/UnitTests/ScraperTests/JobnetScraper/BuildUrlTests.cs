namespace JobScraper.Infrastructure.Tests.UnitTests.ScraperTests.JobnetScraper;

[TestClass]
public class BuildUrlTests
{
    [TestMethod]
    public void BuildUrl_WithSearchTermOnly_ShouldReturnSingleParameterUrl()
    {
        // Arrange
        var url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch";
        var searchTerm = "Systemudvikling, programmering og design";

        // Act
        var result = Scrapers.JobnetScraper.BuildUrl(searchTerm, url);

        // Assert
        var expected =
            "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&SearchString=%2522Systemudvikling,%2520" +
            "programmering%2520og%2520design%2522";

        Assert.AreEqual(expected[0], result[0]);
    }
    
    
}