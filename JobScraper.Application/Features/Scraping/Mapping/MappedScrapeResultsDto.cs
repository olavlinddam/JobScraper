using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Mapping;

public record MappedScrapeResultsDto(
    IEnumerable<JobListing> JobListings,
    IEnumerable<ScrapingError> ScrapingErrors);