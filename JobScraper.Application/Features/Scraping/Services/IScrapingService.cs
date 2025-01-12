using ErrorOr;

namespace JobScraper.Application.Features.Scraping.Services;

public interface IScrapingService
{
    public Task<ErrorOr<Success>> InitiateScrape(CancellationToken cancellationToken);
}