namespace JobScraper.Contracts.Responses.Websites;

public record GetWebsiteWithSearchTermsResponse(
    int Id,
    string Url,
    string ShortName,
    DateTime? LastScraped,
    List<string> SearchTerms);
