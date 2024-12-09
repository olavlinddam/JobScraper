namespace JobScraper.Contracts.Requests.Scraping;

public record ScrapeRequest(string WebsiteBaseUrl, string WebsiteShortName, List<string> SearchTerms);