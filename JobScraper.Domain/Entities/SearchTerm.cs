namespace JobScraper.Domain.Entities;

public class SearchTerm
{
    
    public int Id { get; set; }
    public string Value { get; set; }
    public DateTime LastUsed { get; set; }
    public int MatchingJobsCount { get; set; }
    
    // Navigation
    public int CityId { get; set; }
    public City City { get; set; }
    public ICollection<Website> Websites { get; set; }
    public ICollection<JobListing> JobListings { get; set; }
}