using System.Text.Json;
using JobScraper.Application.Features.Scraping.Services;
using JobScraper.Contracts.Requests.Scraping;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/scraping")]
public class ScrapingController : ApiController
{
    private readonly IScrapingService _scrapingService;

    public ScrapingController(IScrapingService scrapingService)
    {
        _scrapingService = scrapingService;
    }


    [Route("StartAllScrapers")]
    [HttpPost]
    public async Task<IActionResult> StartAllScrapers(StartAllScrapersRequest request)
    {
        var startedAt = DateTime.Now;
        CancellationToken cancellationToken = default;
        
        var result = await _scrapingService.ScrapeAllWebsites(cancellationToken);

        return result.Match(
            success => Ok(new
            {
                message = "Scraping successful!",
                startedAt,
                forceRun = request.ForceRun
            }),
            Problem);
    }
}