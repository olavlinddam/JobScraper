using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces;

public interface IWebsiteRepository
{
    Task<IEnumerable<Website>> GetAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Website>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}