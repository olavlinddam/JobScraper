using JobScraper.Application.Features.CityManagement;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/[controller]")]
public class CityController : ApiController
{
    private readonly ILogger<CityController> _logger;
    private readonly CityService _cityService;

    public CityController(ILogger<CityController> logger, CityService cityService)
    {
        _logger = logger;
        _cityService = cityService;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        CancellationToken cancellationToken = default;
        
        var result = await _cityService.GetAllCities(cancellationToken);
        return result.Match(
            cities => Ok(cities),
            Problem);
    }
}