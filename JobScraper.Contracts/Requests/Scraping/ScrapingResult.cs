namespace JobScraper.Contracts.Requests.Scraping;

public class ScrapingResult
{
    public ScrapedJobData? ScrapedJobData { get; set; }
    public FailedJobScrape? FailedJobScrape { get; set; }
}

public class ScrapedJobData
{
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public DateTime? DatePublished { get; set; }
    public string WorkHours { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string Url { get; set; }
    public string WebsiteBaseUrl { get; set; }
    public string Description { get; set; }

    public string Location { get; set; }
    public string SearchTerm { get; set; }
    public DateTime ScrapedDate { get; set; }
}

public class FailedJobScrape
{
    public string Scraper { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; }
    public string Type { get; set; }
}