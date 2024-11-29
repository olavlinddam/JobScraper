using ErrorOr;
using JobScraper.Application.Common.Interfaces;
using MediatR;

namespace JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;

public class StartAllScrapersCommandHandler : IRequestHandler<StartAllScrapersCommand, ErrorOr<Success>>
{
    private readonly IWebsiteRepository _websiteRepository;

    public StartAllScrapersCommandHandler(IWebsiteRepository websiteRepository)
    {
        _websiteRepository = websiteRepository;
    }

    public async Task<ErrorOr<Success>> Handle(StartAllScrapersCommand request, CancellationToken cancellationToken)
    {
        var websites = await _websiteRepository.GetAllAsync(cancellationToken);

        if (!websites.Any())
        {
            return Error.NotFound(description: "No websites found");
        }

        return Result.Success;
    }
}