using JobScraper.Domain.Entities;

namespace JobScraper.Domain.Tests.UnitTests.SearchTermTests;

[TestClass]
public class SearchTermTests
{
    [TestMethod]
    public void Create_WithValidTerm_ShouldReturnSuccessAndCorrectProperties()
    {
        // Arrange 
        var term = "software developer";

        // Act
        var result = SearchTerm.Create(term);

        // Assert
        Assert.IsFalse(result.IsError);
        Assert.AreEqual(term, result.Value.Value);
        Assert.AreEqual(0, result.Value.MatchingJobsCount);
        Assert.IsNull(result.Value.Websites);
        Assert.IsNull(result.Value.JobListings);
        Assert.IsNull(result.Value.LastUsed);
    }

    [TestMethod]
    public void Create_WithEmptyString_ShouldReturnError()
    {
        // Arrange 
        var term = "";

        // Act
        var result = SearchTerm.Create(term);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("SearchTerm.InvalidTerm", result.Errors[0].Code);
        Assert.AreEqual($"Term '{term}' is invalid.", result.Errors[0].Description);
    }

    [TestMethod]
    public void Create_WithNull_ShouldReturnError()
    {
        // Arrange 
        string term = null;

        // Act
        var result = SearchTerm.Create(term);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("SearchTerm.InvalidTerm", result.Errors[0].Code);
        Assert.AreEqual($"Term '{term}' is invalid.", result.Errors[0].Description);
    }

    [TestMethod]
    public void Create_WithWhitespaceOnly_ShouldReturnError()
    {
        // Arrange 
        var term = "   ";

        // Act
        var result = SearchTerm.Create(term);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("SearchTerm.InvalidTerm", result.Errors[0].Code);
        Assert.AreEqual($"Term '{term}' is invalid.", result.Errors[0].Description);
    }
}