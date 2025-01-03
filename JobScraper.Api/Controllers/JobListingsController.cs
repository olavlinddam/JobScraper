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
    public async Task<IActionResult> GetJobListings()
    {
        CancellationToken cancellationToken = default;
        var result = await _jobListingService.GetJobListings(cancellationToken);

        if (!result.IsError)
        {
            return Ok(result.Value);
        }
        return BadRequest(result.Errors);
    }
}