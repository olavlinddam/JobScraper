using JobScraper.Domain.Entities;

namespace JobScraper.Application.Common.Interfaces.Repositories;

public interface ISearchTermRepository
{
    
    Task AddAsync(SearchTerm searchTerm, CancellationToken cancellationToken);

    Task<List<SearchTerm>> GetAllAsync(CancellationToken cancellationToken);
}