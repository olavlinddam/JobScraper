namespace JobScraper.Domain.Entities;

public class Website
{
    public string Id { get; set; }
    public string Url { get; set; }
    public string ShortName { get; set; }
    public DateTime LastScraped { get; set; }

    
    public int ScrapingPolicyId { get; set; }
    public ScrapingPolicy ScrapingPolicy { get; set; }
    public ICollection<JobListing> JobsListings { get; set; } = new List<JobListing>();
    public ICollection<ScrapingError> ScrapingErrors { get; set; } = new List<ScrapingError>();
}