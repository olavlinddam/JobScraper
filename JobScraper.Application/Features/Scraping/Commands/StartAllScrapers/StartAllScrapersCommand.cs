namespace JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;

public record StartAllScrapersCommand(DateTime StartedAt, bool ForceRun);