using ErrorOr;

namespace JobScraper.Domain.Entities;

public class SearchTerm
{
    public int Id { get; set; }
    public string Value { get; set; }
    public DateTime? LastUsed { get; set; }
    public int MatchingJobsCount { get; set; }

    // Navigation
    public ICollection<Website>? Websites { get; set; }
    public ICollection<JobListing>? JobListings { get; set; }

    private SearchTerm(string value)
    {
        Value = value;
        MatchingJobsCount = 0;
        LastUsed = null;
    }
    
    public static ErrorOr<SearchTerm> Create(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Error.Validation(
                code: "SearchTerm.InvalidTerm",
                description: $"Term '{term}' is invalid."
            );
        }

        return new SearchTerm(term);
    }
}