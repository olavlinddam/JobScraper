using ErrorOr;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Mapping;

public static class WebsiteMapper
{
    public static ErrorOr<Website> MapFromWebsiteRequestToWebsite(AddWebsiteRequest request)
    {
        var result = Website.Create(request.Url, request.ShortName, request.SearchTerms);
        return result;
    }
    public static ErrorOr<Website> MapFromWebsiteRequestToWebsite(AddWebsiteRequest request, List<SearchTerm> searchTerm)
    {
        var result = Website.Create(request.Url, request.ShortName, searchTerm);
        return result;
    }

    public static GetWebsiteResponse MapToWebsiteResponse(Website website)
    {
        return new GetWebsiteResponse
        (
            Id: website.Id,
            ShortName: website.ShortName,
            Url: website.Url,
            LastScraped: website.LastScraped ?? null
        );
    }
}