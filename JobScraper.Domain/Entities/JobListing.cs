using JobScraper.Domain.Enums;


namespace JobScraper.Domain.Entities;

public class JobListing
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public DateTime PostedDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime ScrapedDate { get; set; }
    public JobType JobType { get; set; }
    
    
    // Navigation
    public int WebsiteId { get; set; }
    public Website Website { get; set; }
    
    public int CityId { get; set; }
    public City City { get; set; }
    public ICollection<SearchTerm> SearchTerms { get; set; }
    
}
