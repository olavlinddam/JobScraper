using ErrorOr;
using JobScraper.Application.Common.Interfaces;
using MediatR;

namespace JobScraper.Application.Features.Scraping.Commands.StartAllScrapers;

public class StartAllScrapersCommandHandler(IWebsiteRepository _websiteRepository)
    : IRequestHandler<StartAllScrapersCommand, ErrorOr<Success>>
{
    
    public async Task<ErrorOr<Success>> Handle(StartAllScrapersCommand request, CancellationToken cancellationToken)
    {
        var websites = await _websiteRepository.GetAllAsync(cancellationToken);
        
        
        throw new NotImplementedException();
    }
}