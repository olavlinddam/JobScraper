using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using JobScraper.Domain.Enums;

namespace JobScraper.Application.Features.Scraping.Mapping;

public static class ScrapeResultMapper 
{
    public static List<JobListing> MapToJobListings(List<ScrapedJobData> scrapedJobs, List<City> cities,
        List<Website> websites, List<SearchTerm> searchTerms)
    {
        var jobListings = new List<JobListing>();
        foreach (var scrapedJob in scrapedJobs)
        {
            var website = websites.FirstOrDefault(x => x.Url == scrapedJob.WebsiteBaseUrl);
            var city = cities.FirstOrDefault(city => city.Zip == LocationParser.ExtractZipCode(scrapedJob.Location));
            if (website == null || city == null)
            {
                continue;
            }
            
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
                Website = website,
                City = city,
                SearchTerms = searchTerms.Where(s => s.Value.Contains(scrapedJob.SearchTerm)).ToList()
            };
            jobListings.Add(jobListing);
        }
        return jobListings;
    }


    public static IEnumerable<ScrapingError> MapToScrapingErrors(IEnumerable<FailedJobScrape> failedJobScrapes)
    {
        throw new NotImplementedException();
    }

    public static SearchTerm MapToSearchTerm(ScrapedJobData scrapedJobData, JobListing jobListing)
    {
        var newSearchTerm = new SearchTerm()
        {
            JobListings = new List<JobListing> { jobListing },
            MatchingJobsCount = 1,
            Value = scrapedJobData.SearchTerm,
            Websites = new List<Website> { jobListing.Website },
            LastUsed = DateTime.Now,
        };
        return newSearchTerm;
    }

    public static City MapToCities(ScrapedJobData scrapedJobData)
    {
        return new City
        {
            Name = LocationParser.ExtractCityName(scrapedJobData.Location),
            Zip = LocationParser.ExtractZipCode(scrapedJobData.Location),
            Country = "Denmark",
        };
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