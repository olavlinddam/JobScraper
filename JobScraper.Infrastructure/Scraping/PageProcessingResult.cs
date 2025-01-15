using JobScraper.Application.Features.Scraping.Dtos;
using JobScraper.Infrastructure.Scraping;

namespace JobScraper.Infrastructure.Scrapers;

public record PageProcessingResult(
    bool IsSuccess,
    bool HasNextPage,
    List<ScrapingResult> ScrapingResults);