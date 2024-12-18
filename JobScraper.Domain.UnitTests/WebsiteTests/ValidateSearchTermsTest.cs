using JobScraper.Domain.Entities;

namespace JobScraper.Domain.UnitTests.WebsiteTests;

[TestClass]
public class ValidateSearchTermsTest
{
    [TestMethod]
    public void ValidateSearchTerms_WithEmptyList_ShouldReturnError()
    {
        // Arrange 
        var searchTerms = new List<string>();

        // Act
        var result = Website.ValidateSearchTerms(searchTerms);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual("Website.NoSearchTerms", result.Errors[0].Code);
        Assert.AreEqual("A website must have at least one search term.", result.Errors[0].Description);
    }

    [TestMethod]
    public void ValidateSearchTerms_WithValidTerms_ShouldReturnSuccess()
    {
        // Arrange 
        var searchTerms = new List<string> { "developer", "software engineer" };

        // Act
        var result = Website.ValidateSearchTerms(searchTerms);

        // Assert
        Assert.IsFalse(result.IsError); 
        Assert.AreEqual(2, result.Value.Count); 
    }

    [TestMethod]
    public void ValidateSearchTerms_WithInvalidTerms_ShouldReturnErrors()
    {
        // Arrange 
        var searchTerms = new List<string> { "", "   ", null };

        // Act
        var result = Website.ValidateSearchTerms(searchTerms);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(3, result.Errors.Count); // One error for each invalid term
    }

    [TestMethod]
    public void ValidateSearchTerms_WithMixedValidAndInvalidTerms_ShouldReturnErrors()
    {
        // Arrange 
        var searchTerms = new List<string> { "developer", "", "software engineer" };

        // Act
        var result = Website.ValidateSearchTerms(searchTerms);

        // Assert
        Assert.IsTrue(result.IsError);
        Assert.AreEqual(1, result.Errors.Count); // One error for the empty term
    }
}