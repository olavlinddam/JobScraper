namespace JobScraper.Domain.Entities;

public class TechnologyTag
{
    public int Id { get; set; }
    public string Name { get; set; }

    // Navigation
    public List<JobListing> JobListings { get; set; }
}