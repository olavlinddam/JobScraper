namespace JobScraper.Contracts.Requests.Scraping;

public record ScrapingResult(ScrapedJobData? ScrapedJob, FailedJobScrape? FailedJobScrape);

public record ScrapedJobData(
    string Title,
    string CompanyName,
    string DatePublished,
    string WorkHours,
    string ExpirationDate,
    string Url,
    string Description,
    string Location,
    DateTime ScrapedDate);

public record FailedJobScrape(
    string Scraper,
    DateTime TimeStamp,
    string Message,
    string? StackTrace, 
    string Type);
