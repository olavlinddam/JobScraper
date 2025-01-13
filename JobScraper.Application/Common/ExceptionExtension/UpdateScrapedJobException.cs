namespace JobScraper.Application.Common.ExceptionExtension;

public class UpdateScrapedJobException : Exception
{
    public UpdateScrapedJobException() : base()
    {
    }

    public UpdateScrapedJobException(string message) : base(message)
    {
    }

    public UpdateScrapedJobException(string message, Exception innerException) : base(message, innerException)
    {
    }
}