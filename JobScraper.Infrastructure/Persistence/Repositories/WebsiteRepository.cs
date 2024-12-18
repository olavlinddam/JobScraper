using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Repositories;

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

    public async Task<List<Website>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Websites.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Website>> GetWithSearchTerms(CancellationToken cancellationToken)
    {
         var websites = await _context.Websites
            .Include(w => w.SearchTerms)
            .ToListAsync(cancellationToken);
         
         return websites;
    }

    public async Task<Website?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Websites.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }
}