using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;

namespace JobScraper.Application.Features.ClaudeIntegration;

public interface IClaudeApiClient
{

    Task<List<ClaudeApiJobListingAnalysis>> AnalyzeJobListingsBatch(List<ClaudeApiAnalysisRequest> jobListingBatch);
}
