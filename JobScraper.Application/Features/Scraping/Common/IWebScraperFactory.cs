namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraperFactory
{
    IWebScraper TryCreateWebScraper(string shortName);
}