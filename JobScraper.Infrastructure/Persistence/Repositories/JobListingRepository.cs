using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Repositories;

public class JobListingRepository : IJobListingRepository
{
    private readonly AppDbContext _context;

    public JobListingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(JobListing listing, CancellationToken cancellationToken)
    {
        await _context.AddAsync(listing, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<JobListing> jobListings, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(jobListings, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<JobListing>> GetRecentListingsWithWebsitesAndSearchTerms(CancellationToken cancellationToken)
    {
        var latestListings = await _context.JobListings
            .Where(l => l.ExpirationDate > DateTime.UtcNow)
            .Include(l => l.SearchTerms)
            .Include(l => l.Website)
            .OrderByDescending(l => l.ScrapedDate)
            .Take(100)
            .ToListAsync(cancellationToken);

        return latestListings;
    }

    public async Task UpdateRangeAsync(IEnumerable<JobListing> jobListings, CancellationToken cancellationToken)
    {
        _context.UpdateRange(jobListings);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<JobListing>> GetAllWithCitiesAsync(CancellationToken cancellationToken)
    {
        return await _context.JobListings
            .Include(l => l.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<JobListing>> GetBySearchTextAsync(string searchText, CancellationToken cancellationToken)
    {
        return await _context.JobListings
            .Where(l => l.Description.ToLower().Contains(searchText.ToLower()) ||
                        l.Title.ToLower().Contains(searchText.ToLower()) ||
                        l.ExpirationDate > DateTime.UtcNow)
            .Include(l => l.City)
            .ToListAsync(cancellationToken);
    }
}
