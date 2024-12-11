using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using JobScraper.Domain.Enums;

namespace JobScraper.Application.Features.Scraping.Mappers;

public class ScrapeResultMapper : IScrapeResultMapper
{
    public IEnumerable<JobListing> MapToJobListings(IEnumerable<ScrapedJobData> scrapedJobs, Website website,
        List<City> cities, List<SearchTerm> searchTerms)
    {
        var jobListings = new List<JobListing>();
        foreach (var scrapedJob in scrapedJobs)
        {
            var jobListing = new JobListing
            {
                Title = scrapedJob.Title,
                CompanyName = scrapedJob.CompanyName,
                PostedDate = DateTime.Parse(scrapedJob.DatePublished),
                ExpirationDate = DateTime.Parse(scrapedJob.ExpirationDate),
                Url = scrapedJob.Url,
                Description = scrapedJob.Description,
                ScrapedDate = scrapedJob.ScrapedDate,
                JobType = ParseJobType(scrapedJob.WorkHours),
                Website = website
            };
        }
    }

    public IEnumerable<ScrapingError> MapToScrapingErrors(IEnumerable<FailedJobScrape> failedJobScrapes)
    {
        throw new NotImplementedException();
    }
    
    private static JobType ParseJobType(string workHours)
    {
        return workHours.ToLower() switch
        {
            "fuldtid" => JobType.FullTime,
            "deltid" => JobType.PartTime,
            "midlertidig" => JobType.Temporary,
            "praktik" => JobType.Internship,
            _ => JobType.FullTime  // default
        };
    }
}