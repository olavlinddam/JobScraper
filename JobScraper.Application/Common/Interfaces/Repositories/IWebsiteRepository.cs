using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface IWebsiteRepository
{
    Task AddAsync(Website website, CancellationToken cancellationToken);
    Task<List<Website>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Website>> GetWithSearchTerms(CancellationToken cancellationToken);
    Task<Website?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task DeleteAsync(Website website, CancellationToken cancellationToken);
    Task UpdateAsync(Website matchingWebsite, CancellationToken cancellationToken);
    Task UpdateRangeAsync(List<Website> websites, CancellationToken cancellationToken);
}