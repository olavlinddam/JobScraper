using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Mappers;

public interface IScrapeResultMapper
{
    IEnumerable<ScrapingError> MapToScrapingErrors(IEnumerable<FailedJobScrape> failedJobScrapes);
    SearchTerm MapToSearchTerm(ScrapedJobData scrapedJobData, JobListing jobListing);
    
    City MapToCities(ScrapedJobData scrapedJobData);
    IEnumerable<ScrapedJobData?> MapToJobListings(List<ScrapedJobData> scrapedListingsNotInDb, List<City> cities,
        List<Website> websites, Task<List<SearchTerm>> searchTerms);
}