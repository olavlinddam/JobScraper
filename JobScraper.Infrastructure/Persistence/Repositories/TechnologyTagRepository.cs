using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Repositories;

public class TechnologyTagRepository : ITechnologyTagRepository
{
    private readonly AppDbContext _context;

    public TechnologyTagRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<TechnologyTag>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.TechnologyTags.ToListAsync(cancellationToken);
    }
}