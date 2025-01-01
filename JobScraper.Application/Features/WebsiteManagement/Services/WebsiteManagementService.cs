using System.Data.Common;
using ErrorOr;
using FluentValidation;
using JobScraper.Application.Common.Interfaces.Repositories;
using JobScraper.Application.Features.WebsiteManagement.Mapping;
using JobScraper.Contracts.Requests.Websites;
using JobScraper.Contracts.Responses.Websites;
using JobScraper.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobScraper.Application.Features.WebsiteManagement.Services;

public class WebsiteManagementService : IWebsiteManagementService
{
    private readonly ILogger<WebsiteManagementService> _logger;
    private readonly IWebsiteRepository _websiteRepository;
    private readonly ISearchTermRepository _searchTermRepository;
    private readonly IValidator<UpdateWebsiteRequest> _updateWebsiteRequestValidator;

    public WebsiteManagementService(IWebsiteRepository websiteWebsiteRepository,
        ILogger<WebsiteManagementService> logger, IValidator<UpdateWebsiteRequest> updateWebsiteRequestValidator,
        ISearchTermRepository searchTermRepository)
    {
        _websiteRepository = websiteWebsiteRepository;
        _logger = logger;
        _updateWebsiteRequestValidator = updateWebsiteRequestValidator;
        _searchTermRepository = searchTermRepository;
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

            var existingSearchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);
            var createResult = TryCreateWebsite(request, existingSearchTerms);

            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            var website = createResult.Value;

            await _websiteRepository.AddAsync(website, cancellationToken);
            var websiteResponse = WebsiteMapper.MapToWebsiteResponse(website);

            return websiteResponse;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError("An error occured while saving the website {name}: {e}", request.ShortName, e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while creating new website: {e}", e);
            throw;
        }
    }

    internal static ErrorOr<Website> TryCreateWebsite(AddWebsiteRequest request, List<SearchTerm> existingSearchTerms)
    {
        var matchingSearchTerms = existingSearchTerms.Where(existingSearchTerm =>
            request.SearchTerms.Contains(existingSearchTerm.Value)).ToList();

        ErrorOr<Website> createResult;

        if (matchingSearchTerms.Count != 0)
        {
            createResult = WebsiteMapper.MapFromWebsiteRequestToWebsite(request, matchingSearchTerms);
        }
        else
        {
            createResult = WebsiteMapper.MapFromWebsiteRequestToWebsite(request);
        }

        if (createResult.IsError)
            return createResult.Errors;

        return createResult.Value;
    }

    public async Task<ErrorOr<GetWebsiteResponse>> GetWebsiteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var website = await _websiteRepository.GetByIdAsync(id, cancellationToken);
            if (website == null)
                return Error.NotFound($"Website with id: {id} was not found");

            return WebsiteMapper.MapToWebsiteResponse(website);
        }
        catch (DbException e)
        {
            _logger.LogError("A unexpected database error occured while fetching website: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching website: {e}", e);
            throw;
        }
    }

    public async Task<ErrorOr<GetWebsiteResponse>> UpdateWebsiteAsync(UpdateWebsiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _updateWebsiteRequestValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.ConvertAll(
                    failure => Error.Validation(
                        failure.PropertyName,
                        failure.ErrorMessage));

                return errors;
            }

            var website = await _websiteRepository.GetByIdAsync(request.Id, cancellationToken);
            if (website == null)
                return Error.NotFound($"Website with id: {request.Id} was not found");

            website.UpdateWebsite(request.Url, request.ShortName, request.SearchTerms);

            var websiteResponse = WebsiteMapper.MapToWebsiteResponse(website);
            return websiteResponse;
        }
        catch (DbException e)
        {
            _logger.LogError("A unexpected database error occured while fetching website: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching website: {e}", e);
            throw;
        }
    }

    public async Task<ErrorOr<Success>> DeleteWebsiteAsync(int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var website = await _websiteRepository.GetByIdAsync(id, cancellationToken);
            if (website == null)
                return Error.NotFound($"Website with id: {id} was not found");

            await _websiteRepository.DeleteAsync(website, cancellationToken);
            return Result.Success;
        }
        catch (DbException e)
        {
            _logger.LogError("A unexpected database error occured while fetching website: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching website: {e}", e);
            throw;
        }
    }

    public async Task<ErrorOr<List<GetWebsiteResponse>>> GetAllWebsitesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var websites = await _websiteRepository.GetAllAsync(cancellationToken);
            if (websites.Count == 0)
                return Error.NotFound($"No websites were found");

            return websites.Select(website => WebsiteMapper.MapToWebsiteResponse(website)).ToList();
        }
        catch (DbException e)
        {
            _logger.LogError("A unexpected database error occured while fetching website: {e}", e);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occured while fetching website: {e}", e);
            throw;
        }
    }
}