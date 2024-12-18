using JobScraper.Application.Features.WebsiteManagement.Validation;
using JobScraper.Contracts.Requests.Websites;

namespace Jobscraper.Application.UnitTests.UnitTests.ValidationTests;

[TestClass]
public class AddWebsiteRequestValidatorTests
{
    private readonly AddWebsiteRequestValidator _validator;

    public AddWebsiteRequestValidatorTests()
    {
        _validator = new AddWebsiteRequestValidator();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    public void Validate_EmptyOrNullUrl_ShouldHaveValidationError(string url)
    {
        // Arrange
        var request = new AddWebsiteRequest(
            url,
            "shortname",
            new List<string> { "searchterm" });

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsTrue(result.Errors.Any(x => x.PropertyName == "Url"));
    }

    [TestMethod] 
    public void Validate_ValidRequest_ShouldPassValidation()
    {
        // Arrange
        var request = new AddWebsiteRequest(
            "http://valid.com",
            "shortname",
            new List<string> { "searchterm" });

        // Act
        var result = _validator.Validate(request);

        // Assert
        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }
}