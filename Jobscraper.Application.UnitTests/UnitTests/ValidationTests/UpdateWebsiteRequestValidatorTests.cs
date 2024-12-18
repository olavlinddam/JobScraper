using JobScraper.Application.Features.WebsiteManagement.Validation;
using JobScraper.Contracts.Requests.Websites;

namespace Jobscraper.Application.UnitTests.UnitTests.ValidationTests;

[TestClass]
public class UpdateWebsiteRequestValidatorTests
{
    private readonly UpdateWebsiteRequestValidator _validator;

    public UpdateWebsiteRequestValidatorTests()
    {
        _validator = new UpdateWebsiteRequestValidator();
    }

    [TestMethod]
    public void Validate_AllPropertiesNull_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: null,
            ShortName: null,
            SearchTerms: null);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e =>
            e.ErrorMessage == "At least one property (Url, ShortName, or SearchTerms) must be provided for update"));
    }

    [TestMethod]
    public void Validate_EmptyUrl_WhenUrlProvided_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: "",
            ShortName: null,
            SearchTerms: null);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Url"));
    }

    [TestMethod]
    public void Validate_EmptyShortName_WhenShortNameProvided_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: null,
            ShortName: "",
            SearchTerms: null);

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "ShortName"));
    }

    [TestMethod]
    public void Validate_EmptySearchTermsList_WhenSearchTermsProvided_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: null,
            ShortName: null,
            SearchTerms: new List<string>());

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "SearchTerms"));
    }

    [TestMethod]
    public void Validate_EmptySearchTermInList_WhenSearchTermsProvided_ShouldHaveValidationError()
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: null,
            ShortName: null,
            SearchTerms: new List<string> { "valid", "" });

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(e => e.PropertyName.Contains("SearchTerms")));
    }

    [TestMethod]
    [DataRow("http://valid.com", null, null)]
    [DataRow(null, "validshort", null)]
    [DataRow(null, null, new[] { "term1", "term2" })]
    public void Validate_ValidSinglePropertyUpdate_ShouldPass(string url, string shortName, string[] searchTerms)
    {
        // Arrange
        var request = new UpdateWebsiteRequest(
            Id: 1,
            Url: url,
            ShortName: shortName,
            SearchTerms: searchTerms?.ToList());

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }
}