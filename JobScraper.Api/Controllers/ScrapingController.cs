using JobScraper.Application.Features.Scraping.Services;
using JobScraper.Contracts.Requests.Scraping;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/[controller]")]
public class ScrapingController : ApiController
{
    private readonly IScrapingService _scrapingService;

    public ScrapingController(IScrapingService scrapingService)
    {
        _scrapingService = scrapingService;
    }


    [Route("StartAllScrapers")]
    [HttpPost]
    public async Task<IActionResult> StartAllScrapers()
    {
        var startedAt = DateTime.Now;
        CancellationToken cancellationToken = default;

        await _scrapingService.InitiateScrape(cancellationToken);
        
        return Ok(new
        {
            message = "Scrape cycle complete",
            startedAt,
            finished = DateTime.Now
        });
    }
}