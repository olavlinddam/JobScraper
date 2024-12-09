namespace JobScraper.Domain.Enums;

public enum ScrapingErrorType
{
    RateLimitExceeded,
    ParseError,
    InvalidInput,
    NetworkError,
    WebdriverTimeoutError,
    AuthenticationError
}