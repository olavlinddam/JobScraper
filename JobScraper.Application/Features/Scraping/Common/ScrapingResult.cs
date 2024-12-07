namespace JobScraper.Application.Features.Scraping.Common;

public record ScrapingResult
{
    public ScrapedJobData? ScrapedJob { get; set; }
    public FailedJobScrape? FailedJobScrape { get; set; }
}

public record ScrapedJobData
{
    public string Title { get; set; }
    public string CompanyName { get; set; }
    public DateTime DatePublished { get; set; }
    public string WorkHours { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
    public DateTime ScrapedDate { get; set; }
    public int ZipCode { get; set; }
    public string City { get; set; }
    public string Link { get; set; }
}

public record FailedJobScrape
{
    public DateTime TimeStamp { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Type { get; set; }
}
