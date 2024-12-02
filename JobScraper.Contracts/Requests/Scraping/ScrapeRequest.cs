namespace JobScraper.Contracts.Requests.Scraping;

public record ScrapeRequest
{
    public string? SearchTerm { get; set; }
    public string? Location { get; set; }
    public string? DistanceFromLocation { get; set; }
    public bool FullTimeOnly { get; set; }
}