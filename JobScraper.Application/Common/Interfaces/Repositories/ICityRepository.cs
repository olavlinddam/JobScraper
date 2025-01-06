using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface ICityRepository
{
    
    Task AddAsync(City city, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<City> cities, CancellationToken cancellationToken);

    Task<List<City>> GetAll(CancellationToken cancellationToken);
    Task<List<City>> GetAllWithListings(CancellationToken cancellationToken);
    Task DeleteAll(CancellationToken cancellationToken);
}