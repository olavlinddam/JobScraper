namespace JobScraper.Domain.Entities;

public class ScrapingPolicy
{
    public int Id { get; set; }
    public int RequestsPerMinute { get; set; }
    public bool ShouldRespectRobotsTxt { get; set; }
    public TimeSpan CooldownPeriod { get; set; }
    public List<string> AllowedPaths { get; set; } = new List<string>();
    
    // Navigation
    public ICollection<Website> Websites { get; set; }
}