namespace JobScraper.Domain.Enums;

public enum ScrapingErrorType
{
    RateLimitExceeded,
    ParseError,
    NetworkError,
    WebdriverTimeoutError,
    AuthenticationError
}