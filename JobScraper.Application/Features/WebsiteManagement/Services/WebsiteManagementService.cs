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

    public async Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> CreateWebsiteAsync(AddWebsiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var existingWebsites = await _websiteRepository.GetWithSearchTerms(cancellationToken);
            var matchingWebsite = existingWebsites.FirstOrDefault(x => x.Url == request.Url);
            if (matchingWebsite != null)
                return Error.Conflict("DuplicateWebsiteUrl", $"Website with url {request.Url} already exists");

            var existingSearchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);
            var createResult = TryCreateWebsite(request, existingSearchTerms);

            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            var website = createResult.Value;

            await _websiteRepository.AddAsync(website, cancellationToken);
            var websiteResponse = WebsiteMapper.MapToWebsiteWithSearchTermResponse(website);

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

    internal static ErrorOr<Website> TryCreateWebsite(AddWebsiteRequest request,
        List<SearchTerm> existingSearchTerms)
    {
        var matchingSearchTerms = existingSearchTerms.Where(existingSearchTerm =>
            request.SearchTerms.Contains(existingSearchTerm.Value)).ToList();

        ErrorOr<Website> createResult;

        if (matchingSearchTerms.Count == 0)
        {
            createResult = WebsiteMapper.MapFromWebsiteRequestToWebsite(request);
        }
        else
        {
            createResult = WebsiteMapper.MapFromWebsiteRequestToWebsite(request, matchingSearchTerms);
        }

        return createResult;
    }

    public async Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> GetWebsiteAsync(int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var website = await _websiteRepository.GetByIdAsync(id, cancellationToken);
            if (website == null)
                return Error.NotFound(
                    code: "Website.NotFound",
                    description: $"Website with id: '{id}' was not found");

            return WebsiteMapper.MapToWebsiteWithSearchTermResponse(website);
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


    public async Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> UpdateWebsiteAsync(
        UpdateWebsiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var website = await _websiteRepository.GetByIdAsync(request.Id, cancellationToken);
            if (website is null)
                return Error.NotFound(
                    code: "Website.NotFound",
                    description: $"Website with id: '{request.Id}' was not found");

            var validationResult = ValidateWebsiteUpdateRequest(request);
            if (validationResult.IsError)
                return validationResult.Errors;

            // Update basic details
            var updateDetailsResult = website.UpdateWebsiteDetails(request.Url, request.ShortName);
            if (updateDetailsResult.IsError)
                return updateDetailsResult.Errors;

            // If no search terms to update, we're done
            if (request.SearchTerms is null)
            {
                await _websiteRepository.UpdateAsync(website, cancellationToken);
                return WebsiteMapper.MapToWebsiteWithSearchTermResponse(website);
            }

            // Prepare search terms
            var existingSearchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);
            var newSearchTerms = PrepareSearchTermsForUpdate(request.SearchTerms, existingSearchTerms);
            if (newSearchTerms.IsError)
                return newSearchTerms.Errors;

            // Update search terms (domain logic handles validation)
            var updateSearchTermsResult = website.UpdateSearchTerms(newSearchTerms.Value);
            if (updateSearchTermsResult.IsError)
                return updateSearchTermsResult.Errors;

            await _websiteRepository.UpdateAsync(website, cancellationToken);
            return WebsiteMapper.MapToWebsiteWithSearchTermResponse(website);
        }
        catch (Exception e)
        {
            _logger.LogError("An unexpected error occurred while updating website: {e}", e);
            throw;
        }
    }

    internal static ErrorOr<List<SearchTerm>> PrepareSearchTermsForUpdate(
        List<string> requestedTerms,
        List<SearchTerm> existingSearchTerms)
    {
        var searchTerms = new List<SearchTerm>();

        foreach (var term in requestedTerms)
        {
            var existingTerm = existingSearchTerms.FirstOrDefault(x =>
                x.Value.Equals(term, StringComparison.OrdinalIgnoreCase));

            if (existingTerm != null)
            {
                searchTerms.Add(existingTerm);
            }
            else
            {
                var searchTermCreateResult = SearchTermMapper.MapToSearchTerm(term);
                if (searchTermCreateResult.IsError)
                    return searchTermCreateResult.Errors;

                searchTerms.Add(searchTermCreateResult.Value);
            }
        }

        return searchTerms;
    }

    private ErrorOr<Success> ValidateWebsiteUpdateRequest(UpdateWebsiteRequest request)
    {
        var validationResult = _updateWebsiteRequestValidator.Validate(request);
        if (validationResult.IsValid)
            return Result.Success;

        var errors = validationResult.Errors.ConvertAll(
            failure => Error.Validation(
                failure.PropertyName,
                failure.ErrorMessage));
        return errors;
    }

    public async Task<ErrorOr<Success>> DeleteWebsiteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var website = await _websiteRepository.GetByIdAsync(id, cancellationToken);
            if (website == null)
                return Error.NotFound(
                    code: $"Website.NotFound",
                    description: $"Website with id: '{id}' was not found");

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
                return Error.NotFound(
                    code: "Website.NotFound",
                    description: "No websites were found");

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