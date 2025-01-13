using System.Text.Json.Serialization;
using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;

namespace JobScraper.Application.Features.ClaudeIntegration;

public class ClaudeApiJobListingAnalysis
{
    public int Index { get; set; }
    public ClaudeApiAnalysisResponse ApiAnalysisResponse { get; set; }
    public bool IsSuccess { get; set; }

}