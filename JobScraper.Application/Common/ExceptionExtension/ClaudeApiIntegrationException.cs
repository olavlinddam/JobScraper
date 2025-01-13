namespace JobScraper.Application.Features.ClaudeIntegration;

public class ClaudeApiIntegrationException : Exception
{
    public ClaudeApiIntegrationException() : base()
    {
    }
    
    public ClaudeApiIntegrationException(string message) : base(message)
    {
    }
    
    public ClaudeApiIntegrationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}