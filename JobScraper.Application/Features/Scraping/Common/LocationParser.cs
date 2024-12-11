namespace JobScraper.Application.Features.Scraping.Common;

public static class LocationParser
{
    public static string ExtractCityName(string location)
    {
        if (string.IsNullOrEmpty(location))
            return string.Empty;

        var parts = location.Split(' ');

        return parts.Length == 1 ? parts[0] : parts[1];
    }
}