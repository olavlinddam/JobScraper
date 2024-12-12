using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Mappers;

public interface IScrapeResultMapper
{
    IEnumerable<JobListing> MapToJobListings(IEnumerable<ScrapedJobData?> scrapedJobs);
    IEnumerable<ScrapingError> MapToScrapingErrors(IEnumerable<FailedJobScrape> failedJobScrapes);
    SearchTerm MapToSearchTerm(ScrapedJobData scrapedJobData, JobListing jobListing);
    
    City MapToCities(ScrapedJobData scrapedJobData);
}