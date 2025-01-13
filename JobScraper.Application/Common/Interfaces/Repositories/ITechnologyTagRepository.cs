using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface ITechnologyTagRepository
{
    Task<List<TechnologyTag>> GetAllAsync(CancellationToken cancellationToken);
}