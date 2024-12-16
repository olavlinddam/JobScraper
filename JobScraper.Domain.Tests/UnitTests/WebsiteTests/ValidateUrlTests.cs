using ErrorOr;
using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Tests.UnitTests.WebsiteTests;

[TestClass]
public class ValidateUrlTests
{
    [TestMethod]
    public void ValidateUrl_WithInvalidUrl_ShouldReturnError()
    {
        // Arrange 
        var url = "ww.asd,xpa";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(ErrorType.Validation, result.Errors[0].Type);
        Assert.AreEqual("Website.InvalidUrl", result.Errors[0].Code);
        Assert.AreEqual("Invalid URL format.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateUrl_WithValidUrl_ShouldReturnSuccess()
    {
        // Arrange 
        var url = "https://www.jobnet.dk";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.AreEqual(result.Value, Result.Success);
    }

    [TestMethod]
    public void ValidateUrl_WithEmptyString_ShouldReturnError()
    {
        // Arrange 
        var url = "";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("URL cannot be empty.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateUrl_WithNull_ShouldReturnError()
    {
        // Arrange 
        string url = null;

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("URL cannot be empty.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateUrl_WithOnlyWhitespace_ShouldReturnError()
    {
        // Arrange 
        var url = "   ";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("URL cannot be empty.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateUrl_WithMissingProtocol_ShouldReturnError()
    {
        // Arrange 
        var url = "www.jobnet.dk";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("Invalid URL format.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateUrl_WithQueryParameters_ShouldReturnSuccess()
    {
        // Arrange 
        var url = "https://job.jobnet.dk/CV/FindWork?Offset=0&SortValue=BestMatch&SearchString=developer";

        // Act
        var result = Website.ValidateUrl(url);

        // Assert
        Assert.AreEqual(result.Value, Result.Success);
    }
}