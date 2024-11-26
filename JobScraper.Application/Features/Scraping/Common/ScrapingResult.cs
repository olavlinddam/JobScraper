using OneOf;

namespace JobScraper.Application.Features.Scraping.Common;

public record ScrapingStarted(
    Guid OperationId,
    int ScrapersInitiated,
    DateTime StartedAt
);

public record ScrapingError(
    string Code,
    string Message
);

public class ScrapingResult : OneOfBase<ScrapingStarted, ScrapingError>
{
    public ScrapingResult(OneOf<ScrapingStarted, ScrapingError> input) : base(input)
    {
        Success()
    }

    public static ScrapingResult Success(Guid operationId, int count, DateTime startedAt) =>
        new(new ScrapingStarted(operationId, count, startedAt));

    public static ScrapingResult Error(string code, string message) =>
        new(new ScrapingError(code, message));
}