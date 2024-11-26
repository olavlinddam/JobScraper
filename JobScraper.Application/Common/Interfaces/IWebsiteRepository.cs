namespace JobScraper.Application.Common.Interfaces;

public interface IWebsiteRepository
{
    Task GetAllAsync(CancellationToken cancellationToken);
}