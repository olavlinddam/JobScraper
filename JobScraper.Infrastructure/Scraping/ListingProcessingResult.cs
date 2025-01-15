namespace JobScraper.Infrastructure.Scraping;

public record ListingProcessingResult(
    string Href,
    string Title,
    string CompanyName,
    string DatePublished,
    string ExpirationDate,
    string Location);
