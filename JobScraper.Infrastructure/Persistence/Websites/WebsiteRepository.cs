using JobScraper.Application.Common.Interfaces;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Websites;

public class WebsiteRepository : IWebsiteRepository
{
    private readonly AppDbContext _context;

    public WebsiteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Website website, CancellationToken cancellationToken)
    {
        await _context.AddAsync(website, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Website>> GetAllWithPoliciesAsync(CancellationToken cancellationToken)
    {
        return await _context.Websites.Include(w => w.ScrapingPolicy)
            .ToListAsync(cancellationToken);
    }
}