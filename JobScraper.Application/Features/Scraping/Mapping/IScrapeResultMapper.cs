using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Mappers;

public interface IScrapeResultMapper
{
    IEnumerable<ScrapingError> MapToScrapingErrors(IEnumerable<FailedJobScrape> failedJobScrapes);
    SearchTerm MapToSearchTerm(ScrapedJobData scrapedJobData, JobListing jobListing);
    
    City MapToCities(ScrapedJobData scrapedJobData);
    List<JobListing> MapToJobListings(List<ScrapedJobData> scrapedListingsNotInDb, List<City> cities,
        List<Website> websites, List<SearchTerm> searchTerms);
}