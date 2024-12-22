using JobScraper.Contracts.Requests.Scraping;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraper : IDisposable
{
    Task<List<ScrapingResult>> ScrapePageAsync(ScrapeRequest scrapeRequest, CancellationToken cancellationToken);
}