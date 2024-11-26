using JobScraper.Domain.Enums;

namespace JobScraper.Domain.Entities;

public class JobListing
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Uri { get; set; }
    public string Description { get; set; }
    public string ContactInfo { get; set; }
    public DateTime ScrapedDate { get; set; }
    public JobType JobType { get; set; }
    public City City { get; set; }
    
    
    // Navigation
    public string WebsiteId { get; set; }
    public Website Website { get; set; }
}