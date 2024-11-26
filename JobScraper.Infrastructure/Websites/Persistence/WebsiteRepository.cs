using JobScraper.Application.Common.Interfaces;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Websites.Persistence;

public class WebsiteRepository(AppDbContext _dbContext) : IWebsiteRepository
{
    public async Task<IEnumerable<Website>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Websites.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Website>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var websites = await _dbContext.Websites
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        
        return websites;
    }
}