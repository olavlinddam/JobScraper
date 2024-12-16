using ErrorOr;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.WebsiteManagement.Mapping;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.WebsiteManagement.Services;

public class WebsiteManagementService : IWebsiteManagementService
{
    private readonly ILogger<WebsiteManagementService> _logger;
    private readonly IWebsiteRepository _websiteRepository;

    public WebsiteManagementService(IWebsiteRepository websiteWebsiteRepository, ILogger<WebsiteManagementService> logger)
    {
        _websiteRepository = websiteWebsiteRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<GetWebsiteResponse>> CreateWebsiteAsync(AddWebsiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingWebsites = await _websiteRepository.GetAllAsync(cancellationToken);
            if (existingWebsites.Select(w => w.Url).Contains(request.Url))
            {
                return Error.Conflict("Website already exists");
            }

            var createResult = WebsiteMapper.MapFromWebsiteRequestToWebsite(request);

            if (createResult.IsError)
            {
                return createResult.Errors;
            }
            
            var website = createResult.Value;

            await _websiteRepository.AddAsync(website, cancellationToken);
            var websiteResponse = WebsiteMapper.MapToWebsiteResponse(website);

            return websiteResponse;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while creating new website: {e}", e);
            throw;
        }
    }

    public Task<ErrorOr<Website>> GetWebsiteAsync(int id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}