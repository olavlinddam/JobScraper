namespace JobScraper.Domain.Entities;

public class Website
{
    public string Id { get; set; }
    public string Uri { get; set; }
    public string ShortName { get; set; }
    public DateTime LastScraped { get; set; }
    public ScrapingPolicy ScrapingPolicy { get; set; }
    public ICollection<JobListing> Jobs { get; set; } = new List<JobListing>();
    public ICollection<ScrapingError> ScrapingErrors { get; set; } = new List<ScrapingError>();
}