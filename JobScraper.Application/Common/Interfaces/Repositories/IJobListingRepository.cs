using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface IJobListingRepository
{
    Task AddAsync(JobListing listing, CancellationToken cancellationToken);
    IQueryable<JobListing> GetRecentListings(CancellationToken cancellationToken);
}