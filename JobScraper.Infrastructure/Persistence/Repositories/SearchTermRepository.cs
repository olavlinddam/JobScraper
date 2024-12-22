using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Repositories;

public class SearchTermRepository : ISearchTermRepository
{
    private readonly AppDbContext _context;

    public SearchTermRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(SearchTerm searchTerm, CancellationToken cancellationToken)
    {
        _context.SearchTerms.Add(searchTerm);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SearchTerm>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.SearchTerms.ToListAsync(cancellationToken);
    }
}