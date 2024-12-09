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
            var joblisting = new JobListing();
            joblisting.Title = scrapedJob.Title;
            joblisting.CompanyName = scrapedJob.CompanyName;
            joblisting.PostedDate = DateTime.Parse(scrapedJob.DatePublished);
            joblisting.ExpirationDate = DateTime.Parse(scrapedJob.ExpirationDate);
            joblisting.Url = scrapedJob.Url;
            joblisting.Description = scrapedJob.Description;
            joblisting.ScrapedDate = scrapedJob.ScrapedDate;
            joblisting.JobType = ParseJobType(scrapedJob.WorkHours);
            joblisting.Website = website;
            
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