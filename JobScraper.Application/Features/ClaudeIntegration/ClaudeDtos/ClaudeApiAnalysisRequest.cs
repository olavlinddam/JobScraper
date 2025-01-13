namespace JobScraper.Application.Features.ClaudeIntegration.ClaudeDtos;

public class ClaudeApiAnalysisRequest
{
    public string ArticleMarkdown { get; set; }
    public int Index { get; set; }
    public List<string> Tags { get; set; }
}