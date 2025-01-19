using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;
using JobScraper.Infrastructure.ClaudeApi;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobScraper.IntegrationTests;

[TestClass]
public class ClaudeIntegrationTests
{
    [TestMethod]
    public async Task ClaudeBatchCallIntegrationTest_ShouldReturnTwoAnalysisResponses()
    {
        // Arrange
        var logger = Mock.Of<ILogger<ClaudeApiClient>>();
        var claudeClient = new ClaudeApiClient(new HttpClient(), logger);
        
        var firstListingHtml = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/FirstListing.html"));
        var secondListingHtml = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/SecondListing.html"));
        var tags = new List<string> { "Docker", "ASP.NET", "React", "TypeScript" };
        
        var converter = new ReverseMarkdown.Converter();
        var firstListingRequest = new ClaudeApiAnalysisRequest()
        {
            ArticleMarkdown = converter.Convert(firstListingHtml),
            Index = 1,
            Tags = tags
        };
        var secondListingRequest = new ClaudeApiAnalysisRequest()
        {
            ArticleMarkdown = converter.Convert(secondListingHtml),
            Index = 2,
            Tags = tags
        };

        // Act
        var result = await claudeClient.AnalyzeJobListingsBatch([firstListingRequest, secondListingRequest]);

    }
}
