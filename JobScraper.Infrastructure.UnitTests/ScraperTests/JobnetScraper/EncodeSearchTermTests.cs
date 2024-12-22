namespace JobScraper.Infrastructure.UnitTests.ScraperTests.JobnetScraper;

[TestClass]
public class EncodeSearchTermTests
{
    [TestMethod]
    public void EncodeSearchTerm_SingleWord_ReturnsUnencoded()
    {
        // Arrange
        string input = "Systemadministrator";
        string expectedOutput = "Systemadministrator";

        // Act
        var encoded = Scrapers.JobnetScraper.EncodeSearchTerm(input);

        // Assert
        Assert.AreEqual(expectedOutput, encoded);
    }

    [TestMethod]
    public void EncodeSearchTerm_ComplexPhrase_EncodesCorrectly()
    {
        // Arrange
        string input = "Systemudvikling, programmering og design";
        string expectedOutput = "%2522Systemudvikling,%2520programmering%2520og%2520design%2522";

        // Act
        var encoded = Scrapers.JobnetScraper.EncodeSearchTerm(input);

        // Assert
        Assert.AreEqual(expectedOutput, encoded);
    }
}