namespace JobScraper.Domain.Entities;

public class Website
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string ShortName { get; set; }
    public DateTime LastScraped { get; set; }

    
    // Navigation
    public ICollection<JobListing> JobsListings { get; set; } = new List<JobListing>();
    public ICollection<ScrapingError> ScrapingErrors { get; set; } = new List<ScrapingError>();
    public ICollection<SearchTerm> SearchTerms { get; set; }
}