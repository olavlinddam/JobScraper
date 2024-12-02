namespace JobScraper.Domain.Entities;

public class SearchTerm
{
    public int Id { get; set; }
    public string Value { get; set; }
    public DateTime LastUsed { get; set; }
    public int MatchingJobsCount { get; set; }
    
    // Navigation
    public ICollection<Website> Websites { get; set; }
    public ICollection<JobListing> JobListings { get; set; }
}