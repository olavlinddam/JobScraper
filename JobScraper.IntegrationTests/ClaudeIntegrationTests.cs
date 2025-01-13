using Anthropic.SDK;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;
using JobScraper.Infrastructure.ClaudeApi;
using ReverseMarkdown.Converters;

namespace JobScraper.IntegrationTests;

[TestClass]
public class ClaudeIntegrationTests
{
    [TestMethod]
    public async Task ClaudeBatchCallIntegrationTest_ShouldReturnTwoAnalysisResponses()
    {
        // Arrange
        var claudeClient = new ClaudeApiClient(new HttpClient());
        var converter = new ReverseMarkdown.Converter();
        var firstListingHtml = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/FirstListing.html"));
        var secondListingHtml = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../TestData/SecondListing.html"));
        var tags = new List<string> { "Docker", "ASP.NET", "React", "TypeScript" };
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

        // Assert
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual(1, result.Errors.Count);
    }
}