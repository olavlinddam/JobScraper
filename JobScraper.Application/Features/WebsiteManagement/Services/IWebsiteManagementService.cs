using ErrorOr;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Services;

public interface IWebsiteManagementService
{
    Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> CreateWebsiteAsync(AddWebsiteRequest request,
        CancellationToken cancellationToken);
    
    Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> GetWebsiteAsync(int id, CancellationToken cancellationToken);

    Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> UpdateWebsiteAsync(UpdateWebsiteRequest request, CancellationToken cancellationToken);

    Task<ErrorOr<Success>> DeleteWebsiteAsync(int id, CancellationToken cancellationToken);
    Task<ErrorOr<List<GetWebsiteResponse>>> GetAllWebsitesAsync(CancellationToken cancellationToken);
}