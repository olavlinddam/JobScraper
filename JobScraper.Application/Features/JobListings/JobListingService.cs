using System.Data.Common;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using ErrorOr;
using JobScraper.Application.Features.JobListings.Mapping;
using JobScraper.Contracts.Responses.JobListings;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.JobListings;

public class JobListingService
{
    private readonly ILogger<JobListingService> _logger;
    private readonly IJobListingRepository _jobListingRepository;

    public JobListingService(IJobListingRepository jobListingRepository, ILogger<JobListingService> logger)
    {
        _jobListingRepository = jobListingRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<List<GetJobListingsResponse>>> GetJobListings(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all job listings");
        try
        {
            _logger.LogInformation("should call repo now");
            var jobListings = (await _jobListingRepository.GetAllWithCitiesAsync(cancellationToken)).ToList();
            if (jobListings == null)
            {
                _logger.LogInformation("job listings was null");
            }

            _logger.LogInformation("returned {jobListings.Count} job listings", jobListings.Count);
            _logger.LogInformation("should be done calling repo now");
            if (jobListings.Count == 0)
                return Error.NotFound($"No job listings found");
            
            _logger.LogInformation("should return all job listings");
            return JobListingsMapper.MapToJobListingsResponses(jobListings.ToList());
        }
        catch (DbException e)
        {
            _logger.LogError("A unexpected database error occured while fetching website: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching website: {e}", e);
            throw;
        }
    }
}