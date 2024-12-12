using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.Scraping.Common;
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

    public IQueryable<JobListing> GetRecentListings(CancellationToken cancellationToken)
    {
        var latestListings = _context.JobListings
            .Where(l => l.ExpirationDate > DateTime.Now)
            .Include(l => l.SearchTerms)
            .OrderByDescending(l => l.ScrapedDate)
            .Take(100);
        
        return latestListings;
    }
}