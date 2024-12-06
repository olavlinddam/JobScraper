using ErrorOr;
using JobScraper.Application.Features.Scraping.Models;
using JobScraper.Contracts.Requests.Scraping;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IWebScraper
{
    Task<ErrorOr<List<Models.ScrapingResult>>> ScrapePageAsync(Website website, ScrapeRequest scrapeRequest,
        CancellationToken cancellationToken);
    
}