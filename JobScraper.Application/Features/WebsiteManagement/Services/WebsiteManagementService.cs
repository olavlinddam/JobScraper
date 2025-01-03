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
                return Error.Conflict("Website already exists");

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
                return Error.NotFound($"Website with id: {id} was not found");

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


    public async Task<ErrorOr<GetWebsiteWithSearchTermsResponse>> UpdateWebsiteAsync(UpdateWebsiteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get website and verify it exists
            var website = await _websiteRepository.GetByIdAsync(request.Id, cancellationToken);
            if (website is null)
                return Error.NotFound($"Website with id: {request.Id} was not found");

            var validationResult = ValidateWebsiteUpdateRequest(request);
            if (validationResult.IsError)
                return validationResult.Errors;
            
            // If no search terms to update, simple update
            if (request.SearchTerms is null)
            {
                var updateResult = website.UpdateWebsite(request.Url, request.ShortName, website.SearchTerms.ToList());

                if (updateResult.IsError)
                    return updateResult.Errors;

                await _websiteRepository.UpdateAsync(website, cancellationToken);
                return WebsiteMapper.MapToWebsiteWithSearchTermResponse(website); // Note: Changed to MapToWebsiteResponse
            }

            // Handle search terms update
            var existingSearchTerms = await _searchTermRepository.GetAllAsync(cancellationToken);
            var searchTermsResult = UpdateSearchTerms(website, request.SearchTerms, existingSearchTerms);
            if (searchTermsResult.IsError)
                return searchTermsResult.Errors;

            var updateWithTermsResult = website.UpdateWebsite(request.Url, request.ShortName, searchTermsResult.Value);

            if (updateWithTermsResult.IsError)
                return updateWithTermsResult.Errors;

            await _websiteRepository.UpdateAsync(website, cancellationToken);
            return WebsiteMapper.MapToWebsiteWithSearchTermResponse(website); // Note: Changed to MapToWebsiteResponse
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

    internal static ErrorOr<List<SearchTerm>> UpdateSearchTerms(
        Website website,
        List<string> requestedTerms,
        List<SearchTerm> existingSearchTerms)
    {
        var newTerms = requestedTerms
            .Except(website.SearchTerms.Select(x => x.Value))
            .Except(existingSearchTerms.Select(x => x.Value))
            .ToList();

        var searchTermsToAdd = existingSearchTerms
            .Except(website.SearchTerms.Where(websiteSearchTerm => requestedTerms.Contains(websiteSearchTerm.Value)))
            .Where(existingSearchTerm => requestedTerms.Contains(existingSearchTerm.Value))
            .ToList();

        var newSearchTermsResult = SearchTermMapper.MapRequestSearchTermsToSearchTerms(newTerms);
        if (newSearchTermsResult.IsError)
            return newSearchTermsResult.Errors;

        searchTermsToAdd.AddRange(newSearchTermsResult.Value);
        return searchTermsToAdd;
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

    private async Task<ErrorOr<T>> ExecuteDbOperation<T>(
        Func<CancellationToken, Task<ErrorOr<T>>> operation,
        CancellationToken cancellationToken,
        string operationName)
    {
        try
        {
            return await operation(cancellationToken);
        }
        catch (DbException e)
        {
            _logger.LogError("Database error during {operation}: {error}", operationName, e);
            return Error.Failure("DatabaseError", e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error during {operation}: {error}", operationName, e);
            return Error.Failure("UnexpectedError", e.Message);
        }
    }
}