using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using ErrorOr;
using JobScraper.Application.Features.Scraping.Models;
using JobScraper.Contracts.Requests.Scraping;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScrapingResult = JobScraper.Application.Features.Scraping.Models.ScrapingResult;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IWebScraper
{
    private readonly ChromeOptions _chromeOptions;

    public JobnetScraper()
    {
        _chromeOptions = new ChromeOptions();
        _chromeOptions.AddArgument("headless");
    }

    public async Task<ErrorOr<List<ScrapingResult>>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var url = BuildUrl(website, scrapeRequest);
            var listings = await GetListings(url);
            if (listings.Count == 0)
            {
                return Error.NotFound($"No jobs found for website {website.ShortName}");
            }

            var scrapingResults = new List<ScrapingResult>();
            foreach (var listing in listings)
            {
                var scrapingResult = ParseListing(listing);
                scrapingResults.Add(scrapingResult);
            }

            if (scrapingResults.Count == 0)
                return Error.NotFound("No job listings found");

            return scrapingResults;
        }
        catch (NoSuchElementException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    internal static string BuildUrl(Website website, ScrapeRequest scrapeRequest)
    {
        var baseUrl = website.Url;

        var parameters = new List<string>();

        if (!string.IsNullOrEmpty(scrapeRequest.SearchTerm))
        {
            parameters.Add($"SearchString={EncodeSearchTerm(scrapeRequest.SearchTerm)}");
        }

        if (!string.IsNullOrEmpty(scrapeRequest.Location))
        {
            parameters.Add($"LocationZip={EncodeLocation(scrapeRequest.Location)}");
            if (!string.IsNullOrEmpty(scrapeRequest.DistanceFromLocation))
            {
                parameters.Add($"SearchInGeoDistance={scrapeRequest.DistanceFromLocation}");
            }
        }

        if (scrapeRequest.FullTimeOnly)
        {
            parameters.Add($"WorkHours=Fuldtid");
        }

        return $"{baseUrl}&{string.Join("&", parameters)}";
    }

    internal static string EncodeSearchTerm(string searchTerm)
    {
        if (!searchTerm.Contains(' '))
        {
            return searchTerm;
        }

        // Replace spaces with %2520
        var encodedSpaces = searchTerm.Replace(" ", "%2520");

        // Add %2522 for quotes at start and end
        return $"%2522{encodedSpaces}%2522";
    }

    internal static string EncodeLocation(string location)
    {
        if (!location.Contains(' '))
        {
            return location;
        }

        var firstEncode = Uri.EscapeDataString(location);

        return firstEncode.Replace("%", "%25"); //location.Replace(" ", "%2520");
    }

    private async Task<IReadOnlyCollection<IWebElement>> GetListings(string url)
    {
        using var driver = new ChromeDriver(_chromeOptions);

        await driver.Navigate().GoToUrlAsync(url);
        var listings = driver.FindElements(By.ClassName("job-ad-summary"));
        return listings;
    }

    internal static ScrapingResult ParseListing(IWebElement listing)
    {
        try
        {
            var anchor = listing.FindElement(By.TagName("a"));
            var href = anchor.GetAttribute("href");
            var title = listing.FindElement(By.TagName("h2")).Text;
            var company = listing.FindElement(By.TagName("h3")).Text;
            var description = listing.FindElement(By.CssSelector("div[class='job-ad-external text-small ng-binding']"))
                .Text;

            var jobAdDetails = listing.FindElement(By.ClassName("job-ad-deadlines-inside"));
            var datePublished = jobAdDetails.FindElement(By.CssSelector("span[data-ng-show='item.PostingCreated']"))
                .Text;
            var location = jobAdDetails.FindElement(By.CssSelector("span[data-ng-if='item.AnonymousEmployer']")).Text;
            var workHours = jobAdDetails.FindElement(By.CssSelector("div.job-ad-workhours>span.job-ad-footer-label"))
                .Text;
            var expirationDate = jobAdDetails
                .FindElement(By.CssSelector("div.job-ad-ansogningsfrist>span.job-ad-footer-label")).Text;

            return new ScrapingResult()
            {
                ScrapedJob = new ScrapedJobData
                {
                    Title = title,
                    CompanyName = company,
                    Description = description,
                    City = location.Split(" ").Last(),
                    ZipCode = int.Parse(location.Split(" ").Last()),
                    DatePublished = DateTime.Parse(datePublished),
                    ExpirationDate = DateTime.Parse(expirationDate),
                    ScrapedDate = DateTime.Today,
                    WorkHours = workHours,
                    Link = href
                },
                FailedJobScrape = null
            };
        }
        catch (Exception e)
        {
            return new ScrapingResult()
            {
                ScrapedJob = null,
                FailedJobScrape = new FailedJobScrape
                {
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    TimeStamp = DateTime.Now,
                    Type = "ParseError"
                }
            };
        }
    }
}