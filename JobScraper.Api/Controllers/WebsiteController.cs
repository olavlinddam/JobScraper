using JobScraper.Application.Features.WebsiteManagement.Services;
using JobScraper.Contracts.Requests.Websites;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/website")]
public class WebsiteController : ApiController
{
    private readonly IWebsiteManagementService _websiteManagementService;

    public WebsiteController(IWebsiteManagementService websiteManagementService)
    {
        _websiteManagementService = websiteManagementService;
    }

    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> CreateWebsite(AddWebsiteRequest request)
    {
        CancellationToken cancellationToken = default;
        var result = await _websiteManagementService.CreateWebsiteAsync(request, cancellationToken);

        return result.Match(
            website => CreatedAtAction(
                nameof(GetWebsite),
                new { id = website.Id },
                website),
            Problem);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWebsite(int id)
    {
        CancellationToken cancellationToken = default;
        var result = await _websiteManagementService.GetWebsiteAsync(id, cancellationToken);
        
        return result.Match(
            website => CreatedAtAction(
                nameof(GetWebsite),
                new { id = website.Id },  
                website),
            Problem);
    }
}