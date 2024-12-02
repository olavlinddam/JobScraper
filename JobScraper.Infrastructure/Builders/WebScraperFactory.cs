using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using OpenQA.Selenium.Chrome;

namespace JobScraper.Infrastructure.Builders;

public class WebScraperFactory : IWebScraperFactory
{
    public IWebScraper CreateForWebsite(Website website)
    {
        var baseUrl = website.Url;
        var driver = new ChromeDriver();

        throw new NotImplementedException();
    }
}