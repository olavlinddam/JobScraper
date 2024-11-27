using System.Text.Json;
using JobScraper.Api.Contracts.Requests;
using JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Microsoft.AspNetCore.Components.Route("api/scraping")]
public class ScrapingController(ISender _mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> StartAllScrapers(StartAllScrapersRequest request)
    {
        var startedAt = DateTime.Now;
        var command = new StartAllScrapersCommand(startedAt, request.ForceRun);

        var result = await _mediator.Send(command);

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