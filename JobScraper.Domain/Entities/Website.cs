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

    public void AddSearchTerms(List<SearchTerm> searchTerms)
    {
        var existingSearchTermValues = SearchTerms.Select(s => s.Value).ToList();
        foreach (var searchTerm in searchTerms)
        {
            if (existingSearchTermValues.Contains(searchTerm.Value))
                continue;
            
            SearchTerms.Add(searchTerm);
        }
    }
}