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
            var jobListings = (await _jobListingRepository.GetAllWithCitiesAsync(cancellationToken)).ToList();
            if (jobListings == null)
            {
                _logger.LogInformation("job listings was null");
            }

            _logger.LogInformation("Found {jobListings.Count} job listings", jobListings.Count);
            if (jobListings.Count == 0)
                return Error.NotFound($"No job listings found");

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

    public async Task<ErrorOr<List<GetJobListingsResponse>>> GetJobListingsForCity(string city, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all job listings");
        try
        {
            var jobListings = (await _jobListingRepository.GetAllWithCitiesAsync(cancellationToken)).ToList();
            var citySpecificListings = jobListings.Where(l => l.City.Name.ToLower() == city.ToLower()).ToList();
            if (citySpecificListings.Count == 0)
                return Error.NotFound($"No job listings found");

            return JobListingsMapper.MapToJobListingsResponses(citySpecificListings.ToList());
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

    public async Task<ErrorOr<List<GetJobListingsResponse>>> GetJobListingsBySearchText(string searchText, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all job listings");
        try
        {
            var jobListings = (await _jobListingRepository.GetBySearchTextAsync(searchText, cancellationToken)).ToList();
            if (jobListings.Count == 0)
                return Error.NotFound($"No job listings found");

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