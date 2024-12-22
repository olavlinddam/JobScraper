namespace JobScraper.Contracts.Requests.Websites;

public record UpdateWebsiteRequest(int Id, string? Url, string? ShortName, List<string>? SearchTerms);
