using System.Reflection.Metadata.Ecma335;

namespace JobScraper.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string Zip { get; set; }
    
    
    // Navigation
    public ICollection<JobListing> JobListings { get; set; }
}