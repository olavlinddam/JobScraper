using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces;

public interface IWebsiteRepository
{
    Task AddAsync(Website website, CancellationToken cancellationToken);
    Task<IEnumerable<Website>> GetAllWithPoliciesAsync(CancellationToken cancellationToken);
}