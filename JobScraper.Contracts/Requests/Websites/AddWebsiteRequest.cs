namespace JobScraper.Contracts.Requests.Websites;

public record AddWebsiteRequest(string Url, string ShortName, List<string> SearchTerms);