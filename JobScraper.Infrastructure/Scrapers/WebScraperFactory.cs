using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;
using JobScraper.Infrastructure.Scrapers;
using OpenQA.Selenium.Chrome;

namespace JobScraper.Infrastructure.Builders;

public class WebScraperFactory : IWebScraperFactory
{
    private readonly Dictionary<string, Func<IWebScraper>> _scraperMap = new()
    {
        { "jobnet", () => new JobnetScraper() }
    };

    public IWebScraper CreateForWebsite(Website website)
    {
        if (_scraperMap.TryGetValue(website.ShortName.ToLower(), out var scraper))
        {
            return scraper();
        }

        throw new NotSupportedException($"No scraper found for website: {website.ShortName}");
    }
}