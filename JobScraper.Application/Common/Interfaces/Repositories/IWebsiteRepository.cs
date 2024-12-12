using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface IWebsiteRepository
{
    Task AddAsync(Website website, CancellationToken cancellationToken);
    Task<IEnumerable<Website>> GetWithSearchTerms(CancellationToken cancellationToken);
}