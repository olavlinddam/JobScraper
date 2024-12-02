using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraperFactory
{
    public IWebScraper CreateForWebsite(Website website);
}