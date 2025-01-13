using JobScraper.Application.Features.JobListings;
using Microsoft.AspNetCore.Mvc;

namespace JobScraper.Api.Controllers;

[Route("api/[controller]")]
public class JobListingController : ApiController
{
    private readonly JobListingService _jobListingService;

    public JobListingController(JobListingService jobListingService)
    {
        _jobListingService = jobListingService;
    }

    [HttpGet("job-listings")]
    public async Task<IActionResult> GetJobListings(CancellationToken cancellationToken)
    {
        var result = await _jobListingService.GetJobListings(cancellationToken);

        return result.Match(
            listings => Ok(listings),
            Problem);
    }
    [HttpGet("job-listings/{city}")]
    public async Task<IActionResult> GetJobListingsForCity(string city, CancellationToken cancellationToken)
    {
        var result = await _jobListingService.GetJobListingsForCity(city, cancellationToken);

        return result.Match(
            listings => Ok(listings),
            Problem);
    }

    [HttpGet("job-listings/{searchText}")]
    public async Task<IActionResult> GetJobListings(string searchText, CancellationToken cancellationToken)
    {
        var result = await _jobListingService.GetJobListingsBySearchText(searchText, cancellationToken);
        
        return result.Match(
            listings => Ok(listings),
            Problem);
    }
}