namespace JobScraper.Contracts.Requests.Scraping;

public record StartAllScrapersRequest
{
    public bool ForceRun { get; set; }
}