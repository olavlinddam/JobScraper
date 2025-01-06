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
    public async Task<IActionResult> GetCities(CancellationToken cancellationToken)
    {
        var result = await _cityService.GetAllCities(cancellationToken);
        return result.Match(
            cities => Ok(cities),
            Problem);
    }
 
    [HttpGet("cities/with-jobs")]
    public async Task<IActionResult> GetCitiesWithJobs(CancellationToken cancellationToken)
    {
        var result = await _cityService.GetAllCitiesWithListings(cancellationToken);
        return result.Match(
            citiesWithJobs => Ok(citiesWithJobs),
            Problem);
    }   

    // [HttpDelete("cities")]
    // public async Task<IActionResult> DeleteCities(CancellationToken cancellationToken)
    // {
    //     await _cityService.DeleteCities(cancellationToken);
    //     return Ok();
    // }   
    // [HttpGet("cities/with-jobs")]
    // public async Task<IActionResult> GetCitiesWithJobs(
    //     [FromQuery] int? page,
    //     [FromQuery] int? pageSize,
    //     CancellationToken cancellationToken = default)
    // {
    //     var result = await _cityService.GetAllCitiesWithJobs(page ?? 1, pageSize ?? 10, cancellationToken);
    //     return result.Match(
    //         citiesWithJobs => Ok(citiesWithJobs),
    //         Problem);
    // }
}