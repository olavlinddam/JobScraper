using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.Scraping.Common;

public interface IJobListingRepository
{

    Task AddAsync(JobListing listing, CancellationToken cancellationToken);

    IQueryable<JobListing> GetRecentListings(CancellationToken cancellationToken);
}