namespace JobScraper.Application.Features.Scraping.Common;

public class ScraperStateManager
{
    private readonly HashSet<int> _activeScrapers = [];
    private readonly object _lock = new();

    public bool TryStartScraping(int websiteId)
    {
        lock (_lock)
        {
            if (_activeScrapers.Contains(websiteId))
            {
                return false;
            }

            _activeScrapers.Add(websiteId);
            {
                return true;
            }
        }
    }

    public void StopScraping(int websiteId)
    {
        lock (_lock)
        {
            _activeScrapers.Remove(websiteId);
        }
    }
}