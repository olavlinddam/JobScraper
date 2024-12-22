namespace JobScraper.Contracts.Requests.Scraping;

public record ScrapeRequest(
    string WebsiteBaseUrl, 
    List<string> SearchTerms);