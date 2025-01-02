using ErrorOr;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Mapping;

public static class SearchTermMapper
{
    public static ErrorOr<SearchTerm> MapToSearchTerm(string searchTerm)
    {
        var result = SearchTerm.Create(searchTerm);
        return result;
    }
}
