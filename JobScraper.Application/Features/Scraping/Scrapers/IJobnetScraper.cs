using JobScraper.Application.Features.Scraping.Common;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;
using ErrorOr;

namespace JobScraper.Application.Features.Scraping.Scrapers;

public interface IJobnetScraper 
{
    Task<ErrorOr<List<ScrapingResult>>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest, CancellationToken cancellationToken);
}