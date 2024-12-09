using ErrorOr;
using JobScraper.Application.Features.Scraping.Common;

namespace JobScraper.Application.Features.Scraping.Services;

public interface IScrapingService
{
    public Task<ErrorOr<Success>> InitiateScrape(CancellationToken cancellationToken);
}