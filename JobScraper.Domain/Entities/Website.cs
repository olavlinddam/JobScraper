using ErrorOr;

namespace JobScraper.Domain.Entities;

public class Website
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string ShortName { get; set; }
    public DateTime LastScraped { get; set; }

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
    }

    public static ErrorOr<Website> Create(string url, string shortName, List<string> searchTerms)
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

        var validSearchTerms = new List<SearchTerm>();
        if (searchTerms.Count == 0)
        {
            errors.Add(Error.Validation(
                code: "Website.NoSearchTerms",
                description: "A website must have at least one search term."));
        }
        else
        {
            foreach (var term in searchTerms)
            {
                var termResult = SearchTerm.Create(term);
                if (termResult.IsError)
                {
                    errors.AddRange(termResult.Errors.Select(e => Error.Validation(
                        code: "Website.InvalidSearchTerm",
                        description: $"Search term '{term}' is invalid: {e.Description}")));
                }
                else
                {
                    validSearchTerms.Add(termResult.Value);
                }
            }
        }

        if (errors.Count != 0)
        {
            return errors;
        }

        var website = new Website(url, shortName, validSearchTerms);
        return website;
    }

    public ErrorOr<Success> AddSearchTerms(IEnumerable<string> terms)
    {
        var errors = new List<Error>();
        var createdSearchTerms = new List<SearchTerm>();

        foreach (var term in terms)
        {
            var createSearchTermResult = SearchTerm.Create(term);
            if (createSearchTermResult.IsError)
            {
                errors.AddRange(createSearchTermResult.Errors);
            }
            else
            {
                createdSearchTerms.Add(createSearchTermResult.Value);
            }

            if (errors.Count != 0)
            {
                return errors; // return all accumulated errors
            }

            // If all terms are valid, add them to the website
            foreach (var searchTerm in createdSearchTerms)
            {
                SearchTerms.Add(searchTerm);
            }
        }

        return Result.Success;
    }
}