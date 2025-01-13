using System.Text.Json.Serialization;

namespace JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;

public class ClaudeApiAnalysisResponse
{
    [JsonPropertyName("yearsOfExperience")] public int YearsOfExperience { get; set; }
    [JsonPropertyName("tags")] public List<string> tags { get; set; }
    [JsonPropertyName("language")] public string LanguageCode { get; set; }
    [JsonPropertyName("summary")] public string Summary { get; set; }
}