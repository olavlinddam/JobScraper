namespace JobScraper.Application.Features.Scraping.Common;

public record ScrapingResult
{
    public bool Success { get; }
    public string? Content { get; }
    public string? Error { get; }

    private ScrapingResult(bool success, string? content = null, string? error = null)
    {
        Success = success;
        Content = content;
        Error = error;
    }

    public static ScrapingResult Succeeded(string content) =>
        new(true, content);

    public static ScrapingResult Failed(string error) =>
        new(false, error: error);
}