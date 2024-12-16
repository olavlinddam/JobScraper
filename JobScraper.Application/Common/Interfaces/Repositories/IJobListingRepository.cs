using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface IJobListingRepository
{

    Task AddAsync(JobListing listing, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<JobListing> jobListings, CancellationToken cancellationToken);

    Task<List<JobListing>> GetRecentListingsWithWebsitesAndSearchTerms(CancellationToken cancellationToken);
    Task UpdateRangeAsync(IEnumerable<JobListing> jobListings, CancellationToken cancellationToken);
}