using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using JobScraper.

using ErrorOr;
using JobScraper.Contracts.Requests.Scraping;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IWebScraper
{
    private readonly IWebDriver _driver = new ChromeDriver(); 
    public Task<ErrorOr<string>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest, CancellationToken cancellationToken)
    {
        var url = BuildUrl(website, scrapeRequest);
    }

    internal static string BuildUrl(Website website, ScrapeRequest scrapeRequest)
    {
        var baseUrl = website.Url;

        var parameters = new List<string>();

        if (!string.IsNullOrEmpty(scrapeRequest.SearchTerm))
        {
            parameters.Add($"SearchString={EncodeSe}");
        }
    }
}