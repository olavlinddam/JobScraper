using JobScraper.Application.Features.WebsiteManagement.Services;
using JobScraper.Contracts.Requests.Websites;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/[controller]")]
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

    [Route("update")]
    [HttpPut]
    public async Task<IActionResult> UpdateWebsite(UpdateWebsiteRequest request)
    {
        CancellationToken cancellationToken = default;
        var result = await _websiteManagementService.UpdateWebsiteAsync(request, cancellationToken);

        return result.Match(
            website => Ok(website),
            Problem);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWebsite(int id)
    {
        CancellationToken cancellationToken = default;
        var result = await _websiteManagementService.DeleteWebsiteAsync(id, cancellationToken);

        return result.Match(
            _ => Ok(new
            {
                message = $"Successfully deleted website with id {id}.",
            }),
            Problem);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetWebsite(int id)
    {
        CancellationToken cancellationToken = default;
        var result = await _websiteManagementService.GetWebsiteAsync(id, cancellationToken);

        return result.Match(
            website => Ok(website),
            Problem);
    }
}