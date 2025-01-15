namespace JobScraper.Application.Features.Scraping.Dtos;

public record ScrapeRequest(
    string WebsiteBaseUrl, 
    string SearchTerm,
    string? LatestScrapedUrl);