namespace JobScraper.Contracts.Responses.Websites;

public record GetWebsiteResponse(
    int Id,
    string Url,
    string ShortName,
    DateTime? LastScraped
);
