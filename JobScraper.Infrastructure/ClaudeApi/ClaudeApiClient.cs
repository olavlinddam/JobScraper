using System.Text.Json;
using Anthropic.SDK;
using Anthropic.SDK.Batches;
using Anthropic.SDK.Constants;
using Anthropic.SDK.Messaging;
using JobScraper.Application.Features.ClaudeIntegration;
using JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;
using Microsoft.Extensions.Logging;

namespace JobScraper.Infrastructure.ClaudeApi;

public class ClaudeApiClient : IClaudeApiClient
{
    private readonly AnthropicClient _client;
    private readonly ILogger<ClaudeApiClient> _logger;

    public ClaudeApiClient(HttpClient httpClient, ILogger<ClaudeApiClient> logger)
    {
        _logger = logger;
        _client = new AnthropicClient("",
            httpClient);
    }

    public async Task<List<ClaudeApiJobListingAnalysis>> AnalyzeJobListingsBatch(
        List<ClaudeApiAnalysisRequest> jobListingBatch)
    {
        try
        {
            _logger.LogInformation("Initiating ClaudeAPI job listing analysis batch.");
            var batchRequests = CreateBatchRequest(jobListingBatch);
            var batch = await _client.Batches.CreateBatchAsync(batchRequests);
            await PollForCompletion(batch);
            var processedResults = await ProcessBatchResults(batch);
            return processedResults;
        }
        catch (Exception e)
        {
            throw new ClaudeApiIntegrationException(e.Message, e.InnerException ?? e);
        }
    }

    private async Task<List<ClaudeApiJobListingAnalysis>> ProcessBatchResults(BatchResponse batch)
    {
        var results = new List<ClaudeApiJobListingAnalysis>();
        await foreach (var result in _client.Batches.RetrieveBatchResultsAsync(batch.Id))
        {
            try
            {
                results.Add(new ClaudeApiJobListingAnalysis
                {
                    Index = int.Parse(result.CustomId),
                    ApiAnalysisResponse = JsonSerializer.Deserialize<ClaudeApiAnalysisResponse>(result.Result.Message.Message)!,
                    IsSuccess = result.Result.Type == "succeeded"
                });
            }
            catch (JsonException)
            {
                results.Add(new ClaudeApiJobListingAnalysis
                {
                    Index = int.Parse(result.CustomId),
                    ApiAnalysisResponse = new ClaudeApiAnalysisResponse { LanguageCode = "da", tags = ["failed"], YearsOfExperience = 0 },
                    IsSuccess = false
                });
            }
        }

        return results;
    }

    private async Task PollForCompletion(BatchResponse batch)
    {
        BatchResponse status;
        do
        {
            _logger.LogInformation("Polling for batch completion...");
            await Task.Delay(TimeSpan.FromSeconds(30)); // Poll every 30 seconds
            status = await _client.Batches.RetrieveBatchStatusAsync(batch.Id);
        } while (status.ProcessingStatus == "in_progress");

        if (status.ProcessingStatus != "ended")
        {
            throw new ClaudeApiIntegrationException("An error occured while polling for batch completion.");
        }
    }

    private static List<BatchRequest> CreateBatchRequest(List<ClaudeApiAnalysisRequest> jobListingBatch)
    {
        return jobListingBatch.Select(listing =>
        {
            var messages = new List<Message>
            {
                new(RoleType.User,
                    $$"""
                      You're a Job Listing Analyzer AI. Analyze this job listing and output in JSON format with these exact keys and format:
                      {
                          "yearsOfExperience": integer, // Minimum years required (see experience rules) 
                          "tags": string[],             // Array of technology tags following rules below
                          "language": string            // MUST be ISO 639-1 code like "en", "da"
                          "summary" : string            // // Concise summary of the role and requirements
                      }

                      Tag formatting rules:
                      1. If technology matches this list (case-insensitive), use exact tag: {{listing.Tags}}
                      2. For new technologies:
                         - lowercase only
                         - only dots and hyphens allowed
                         - use consistent names (e.g. "react" not "reactjs")
                         - include version numbers (e.g. "python3.8")
                      3. Standard formats:
                         - frameworks: lowercase with dots ("asp.net")
                         - languages: lowercase, no suffix ("python")
                         - databases: lowercase ("postgresql")

                      Experience rules:
                      1. For explicit requirements:
                      - Use the stated minimum years (e.g., 3 for ""3-5 years"")
                      - Remove any ""+""
                      - Round down to nearest integer
                      2. For implicit requirements:
                      - Senior/Lead/Principal roles: 5
                      - Mid-level/Regular roles: 3
                      - Junior roles: 0
                      - Graduate/Entry roles: 0
                      3. Keywords that indicate non-junior:
                      - ""Not suitable for juniors"": 3
                      - ""Experienced"": 3
                      - ""Seasoned professional"": 5
                      4. Use 0 only when:
                      - Explicitly stated as entry-level/no experience required
                      - Junior position clearly indicated
                      5. If experience level cannot be determined with confidence: -1

                      Language rules:
                      1. Must be ISO 639-1 code (e.g. "en", "de")

                      Summary rules:
                      1. Maximum 150 characters
                      2. Focus on key responsibilities and main technologies
                      3. Use present tense
                      4. Start with a verb (e.g., ""Develop"", ""Build"", ""Maintain"")
                      5. Include seniority level if specified
                      6. Exclude company information and benefits
                      7. Don't repeat years of experience
                      8. Remove buzzwords and marketing language

                      IMPORTANT: Respond with ONLY the JSON object. No text before or after.

                      Job listing to analyze:
                      {{listing.ArticleMarkdown}}
                      """)
            };
            return new BatchRequest
            {
                CustomId = listing.Index.ToString(),
                MessageParameters = new MessageParameters
                {
                    Messages = messages,
                    MaxTokens = 1024,
                    Model = AnthropicModels.Claude3Haiku,
                    Temperature = 0m // Lower temperature for more consistent JSON output
                }
            };
        }).ToList();
    }
}