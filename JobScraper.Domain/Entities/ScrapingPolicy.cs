namespace JobScraper.Domain.Entities;

public class ScrapingPolicy
{
    public string Id { get; set; }
    public int RequestsPerMinute { get; set; }
    public bool ShouldRespectRobotsTxt { get; set; }
    public TimeSpan CooldownPeriod { get; set; }
    public List<string> AllowedPaths { get; set; } = new List<string>();
}