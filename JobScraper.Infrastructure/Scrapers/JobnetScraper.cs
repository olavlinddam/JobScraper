using ErrorOr;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IWebScraper
{
    private readonly IWebDriver _driver = new ChromeDriver();
    private readonly string _baseUrl = ""
    public Task<ErrorOr<string>> ScrapePageAsync(string url, ScrapingPolicy policy, CancellationToken cancellationToken)
    {
        
    }
}