using ErrorOr;
using MediatR;

namespace JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;

public record StartAllScrapersCommand(DateTime StartedAt, bool ForceRun) : IRequest<ErrorOr<Success>>;