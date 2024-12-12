using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Mapping;

public static class WebsiteMapper
{
    public static Website MapFromWebsiteRequestToWebsite(AddWebsiteRequest request)
    {
        return new Website
        {
            Url = request.Url,
            ShortName = request.ShortName,
            SearchTerms = request.SearchTerms.Select(st => new SearchTerm { Value = st }).ToList(),
        };
    }

    public static GetWebsiteResponse MapToWebsiteResponse(Website website)
    {
        return new GetWebsiteResponse
        (
            Id: website.Id,
            ShortName: website.ShortName,
            Url: website.Url,
            LastScraped: website.LastScraped
        );
    }
}