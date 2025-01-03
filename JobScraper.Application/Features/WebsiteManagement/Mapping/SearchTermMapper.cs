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
    public static ErrorOr<List<SearchTerm>> MapRequestSearchTermsToSearchTerms(List<string> searchTerms)
    {
        var newSearchTerms = new List<SearchTerm>();
        foreach (var result in searchTerms.Select(SearchTerm.Create))
        {
            if (result.IsError)
                return result.Errors;
            newSearchTerms.Add(result.Value);
        }
        return newSearchTerms;
    }
}
