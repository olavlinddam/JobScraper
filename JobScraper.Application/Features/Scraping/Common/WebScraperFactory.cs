using JobScraper.Application.Features.Scraping.Scrapers;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.Scraping.Common;

public class WebScraperFactory : IWebScraperFactory
{
    private readonly ILogger<WebScraperFactory> _logger;
    private readonly Dictionary<string, Func<IWebScraper>> _scrapers;
    
    public WebScraperFactory(ILogger<WebScraperFactory> logger, IJobnetScraper jobnetScraper)
    {
        _logger = logger;

        _scrapers = new Dictionary<string, Func<IWebScraper>>
        {
            ["jobnet"] = () => jobnetScraper,
        };
    }
    
    public IWebScraper TryCreateWebScraper(string shortName)
    {
        _logger.LogInformation("Trying to create web scraper for {shortName}", shortName);
        if (!_scrapers.TryGetValue(shortName, out var scraper))
            throw new KeyNotFoundException($"The given key {shortName} did not match an existing web scraper."); 
        
        return scraper.Invoke();
    }
}