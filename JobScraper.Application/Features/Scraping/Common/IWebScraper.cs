using ErrorOr;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraper
{
    Task<ErrorOr<string>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken);
    
}