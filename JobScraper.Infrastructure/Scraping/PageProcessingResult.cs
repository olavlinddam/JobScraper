using JobScraper.Application.Features.Scraping.Dtos;

namespace JobScraper.Infrastructure.Scrapers;

public record PageProcessingResult(
    bool IsSuccess,
    bool HasNextPage,
    List<ScrapingResult> ScrapingResults);
