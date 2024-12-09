using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Mappers;

public record MappedScrapeResultsDto(
    IEnumerable<JobListing> JobListings,
    IEnumerable<ScrapingError> ScrapingErrors);