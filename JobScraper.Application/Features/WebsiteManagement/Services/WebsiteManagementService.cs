using ErrorOr;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.WebsiteManagement.Mapping;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;

namespace JobScraper.Application.Features.WebsiteManagement.Services;

public class WebsiteManagementService : IWebsiteManagementService
{
    private readonly IWebsiteRepository _websiteRepository;

    public WebsiteManagementService(IWebsiteRepository websiteWebsiteRepository)
    {
        _websiteRepository = websiteWebsiteRepository;
    }

    public async Task<ErrorOr<GetWebsiteResponse>> CreateWebsiteAsync(AddWebsiteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingWebsites = await _websiteRepository.GetAllAsync(cancellationToken);
            if (existingWebsites.Select(w => w.Url).Contains(request.Url))
            {
                return Error.Conflict("Website already exists");
            }
            
            var website = WebsiteMapper.MapFromWebsiteRequestToWebsite(request);
            // TODO: Validate...
            
            await _websiteRepository.AddAsync(website, cancellationToken);
            var websiteResponse = WebsiteMapper.MapToWebsiteResponse(website);
            
            return websiteResponse;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<ErrorOr<Website>> GetWebsiteAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}