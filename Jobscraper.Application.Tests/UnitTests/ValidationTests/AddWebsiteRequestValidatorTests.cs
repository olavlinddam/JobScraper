using JobScraper.Application.Features.WebsiteManagement.Validation;
using JobScraper.Contracts.Requests.Websites;

namespace Jobscraper.Application.Tests.UnitTests.ValidationTests;

[TestClass]
public class AddWebsiteRequestValidatorTests
{
     private readonly AddWebsiteRequestValidator _validator;
    
        public AddWebsiteRequestValidatorTests()
        {
            _validator = new AddWebsiteRequestValidator();
        }
    
        [Theory]
        [InlineData("")]
        [InlineData(null)]
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
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Url");
        }
}