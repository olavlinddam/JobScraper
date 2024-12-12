using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobScraper.Infrastructure.Persistence.Repositories;

public class CityRepository : ICityRepository
{
    private readonly AppDbContext _context;

    public CityRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(City city, CancellationToken cancellationToken)
    {
        await _context.AddAsync(city, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<City> cities, CancellationToken cancellationToken)
    {
        await _context.AddRangeAsync(cities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<City>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Cities.ToListAsync(cancellationToken);
    }
}