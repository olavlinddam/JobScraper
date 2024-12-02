using ErrorOr;
using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Domain.Entities;

namespace JobScraper.Infrastructure.Scrapers;

public class JobnetScraper : IWebScraper
{
    public Task<ErrorOr<string>> ScrapePageAsync(string url, ScrapingPolicy policy, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}