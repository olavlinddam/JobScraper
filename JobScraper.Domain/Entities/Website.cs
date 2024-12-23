using System.Text.RegularExpressions;
using ErrorOr;

namespace JobScraper.Domain.Entities;

public class Website
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string ShortName { get; set; }
    public DateTime? LastScraped { get; set; }

    // Navigation
    public ICollection<JobListing> JobsListings { get; set; } = new List<JobListing>();
    public ICollection<ScrapingError> ScrapingErrors { get; set; } = new List<ScrapingError>();
    public ICollection<SearchTerm> SearchTerms { get; set; }

    protected Website() // FOR EF CORE
    {
        Url = string.Empty;
        ShortName = string.Empty;
        JobsListings = new List<JobListing>();
        ScrapingErrors = new List<ScrapingError>();
        SearchTerms = new List<SearchTerm>();
    }

    private Website(string url, string shortName, List<SearchTerm> searchTerms)
    {
        Url = url;
        ShortName = shortName;
        SearchTerms = searchTerms;
        LastScraped = null;
    }

    public static ErrorOr<Website> Create(string url, string shortName, List<string> searchTerms)
    {
        var errors = new List<Error>();

        var urlValidationErrors = ValidateUrl(url);
        if (urlValidationErrors.IsError)
            errors.AddRange(urlValidationErrors.Errors);

        var shortnameValidationErrors = ValidateShortName(shortName);
        if (shortnameValidationErrors.IsError)
            errors.AddRange(shortnameValidationErrors.Errors);

        var searchTermValidationResult = ValidateSearchTerms(searchTerms);
        if (searchTermValidationResult.IsError)
            errors.AddRange(searchTermValidationResult.Errors);

        var validSearchTerms = searchTermValidationResult.Value;

        if (errors.Count != 0)
        {
            return errors;
        }

        var website = new Website(url, shortName, validSearchTerms);
        return website;
    }

    internal static ErrorOr<Success> ValidateShortName(string shortName)
    {
        var errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(shortName))
            errors.Add(Error.Validation(code: "Website.InvalidShortName", description: "ShortName cannot be empty."));
        
        var match = Regex.Match(shortName, @"^[a-zA-Z0-9_\-\.]+$");
        if (!match.Success)
            errors.Add(Error.Validation(code: "Website.InvalidShortName", description: $"ShortName is invalid {match.Value}"));
        
        if (errors.Count != 0)
            return errors;

        return Result.Success;
    }

    internal static ErrorOr<List<SearchTerm>> ValidateSearchTerms(List<string> searchTerms)
    {
        var errors = new List<Error>();
        var validSearchTerms = new List<SearchTerm>();

        if (searchTerms.Count == 0)
        {
            return Error.Validation(
                code: "Website.NoSearchTerms",
                description: "A website must have at least one search term.");
        }

        foreach (var term in searchTerms)
        {
            var searchTermResult = SearchTerm.Create(term);
            if (searchTermResult.IsError)
            {
                errors.AddRange(searchTermResult.Errors.Select(e => Error.Validation(
                    code: "Website.InvalidSearchTerm",
                    description: $"Search term '{term}' is invalid: {e.Description}")));
            }
            else
            {
                validSearchTerms.Add(searchTermResult.Value);
            }
        }

        if (errors.Count != 0)
            return errors;

        return validSearchTerms;
    }

    internal static ErrorOr<Success> ValidateUrl(string url)
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(url))
        {
            errors.Add(Error.Validation(code: "Website.InvalidUrl", description: "URL cannot be empty."));
        }
        else if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            errors.Add(Error.Validation(code: "Website.InvalidUrl", description: "Invalid URL format."));
        }

        if (errors.Count != 0)
            return errors;

        return Result.Success;
    }

    public ErrorOr<Website> UpdateWebsite(string? newUrl, string? newShortName, List<string>? newSearchTerms)
    {
        var errors = new List<Error>();

        if (newUrl != null)
        {
            var urlValidationErrors = ValidateUrl(newUrl);
            if (urlValidationErrors.IsError)
                errors.AddRange(urlValidationErrors.Errors);
        }

        if (newShortName != null)
        {
            var shortnameValidationErrors = ValidateShortName(newShortName);
            if (shortnameValidationErrors.IsError)
                errors.AddRange(shortnameValidationErrors.Errors);
        }

        if (newSearchTerms != null)
        {
            var searchTermValidationResult = ValidateSearchTerms(newSearchTerms);
            if (searchTermValidationResult.IsError)
                errors.AddRange(searchTermValidationResult.Errors);
            var validSearchTerms = searchTermValidationResult.Value;
            SearchTerms.Clear();
            SearchTerms = validSearchTerms;
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        return this;
    }
}