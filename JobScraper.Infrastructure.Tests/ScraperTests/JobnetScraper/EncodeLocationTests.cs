namespace JobScraper.Infrastructure.Tests.ScraperTests.JobnetScraper;

[TestClass]
public class EncodeLocationTests
{
    [TestMethod]
    public void EncodeLocation_WithSingleWord_ReturnUnencoded()
    {
        // Arrange 
        var location = "Odense";
        
        // Act 
        var result = Scrapers.JobnetScraper.EncodeLocation(location);
        
        // Assert
        Assert.AreEqual("Odense", result);
    }

    [TestMethod]
    public void EncodeLocation_WithPostalCode_EncodesSpaces()
    {
        // Arrange
        var location = "5000 Odense C";
        
        // Act
        var result = Scrapers.JobnetScraper.EncodeLocation(location);
        
        // Assert
        Assert.AreEqual("5000%2520Odense%2520C", result);
    }

    [TestMethod]
    public void EncodeLocation_WithDanishCharacters_EncodesCorrectly()
    {
        // Arrange
        var location = "Århus Ø";
        
        // Act
        var result = Scrapers.JobnetScraper.EncodeLocation(location);
        
        // Assert
        Assert.AreEqual("%25C3%2585rhus%2520%25C3%2598", result);
    }
}