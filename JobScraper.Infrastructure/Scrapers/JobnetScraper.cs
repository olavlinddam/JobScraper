using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using ErrorOr;
using JobScraper.Contracts.Requests.Scraping;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IWebScraper
{
    private readonly IWebDriver _driver = new ChromeDriver();

    public Task<ErrorOr<string>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken)
    {
        var url = BuildUrl(website, scrapeRequest);

        _driver.Navigate().GoToUrl(url);
        return null;
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

        return firstEncode.Replace("%", "%25");  //location.Replace(" ", "%2520");
    }
}