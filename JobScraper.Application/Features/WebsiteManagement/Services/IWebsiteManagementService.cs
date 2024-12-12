using ErrorOr;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Services;

public interface IWebsiteManagementService
{
    Task<ErrorOr<GetWebsiteResponse>> CreateWebsiteAsync(AddWebsiteRequest request, CancellationToken cancellationToken);
    
    Task<ErrorOr<Website>> GetWebsiteAsync(int id, CancellationToken cancellationToken);
}