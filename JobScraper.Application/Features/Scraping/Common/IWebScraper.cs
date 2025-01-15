using JobScraper.Application.Features.Scraping.Dtos;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraper : IDisposable
{
    Task<List<ScrapingResult>> ScrapeAsync(ScrapeRequest scrapeRequest, CancellationToken cancellationToken);
}