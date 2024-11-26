using ErrorOr;
using MediatR;

namespace JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;

public class StartAllScrapersCommandHandler(IWebsiteRepository _websiteRepository)
    : IRequestHandler<StartAllScrapersCommand, ErrorOr<Success>>
{
    
    public Task<ErrorOr<Success>> Handle(StartAllScrapersCommand request, CancellationToken cancellationToken)
    {
        var websites = _websiteRepository.GetAllAsync();
        
        
        throw new NotImplementedException();
    }
}