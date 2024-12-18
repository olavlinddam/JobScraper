using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface IWebsiteRepository
{
    Task AddAsync(Website website, CancellationToken cancellationToken);
    Task<List<Website>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Website>> GetWithSearchTerms(CancellationToken cancellationToken);
    Task<Website?> GetByIdAsync(int id, CancellationToken cancellationToken);
}