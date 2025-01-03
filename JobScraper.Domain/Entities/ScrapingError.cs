using JobScraper.Domain.Enums;

namespace JobScraper.Domain.Entities;

public class ScrapingError
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public int RetryCount { get; set; }
    public ScrapingErrorType ErrorType { get; set; }
    
    
    // Navigation
    public ICollection<Website> Websites { get; set; }
}