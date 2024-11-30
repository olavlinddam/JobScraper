using ErrorOr;

using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraper
{
    Task<ErrorOr<string>> ScrapePageAsync(string url, ScrapingPolicy policy, CancellationToken cancellationToken);
}