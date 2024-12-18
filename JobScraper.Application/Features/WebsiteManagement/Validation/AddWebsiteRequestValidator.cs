using FluentValidation;
using JobScraper.Contracts.Requests.Websites;

namespace JobScraper.Application.Features.WebsiteManagement.Validation;

public class AddWebsiteRequestValidator : AbstractValidator<AddWebsiteRequest>
{
    public AddWebsiteRequestValidator()
    {
        // Basic structural validation
        RuleFor(x => x.Url)
            .NotEmpty();

        RuleFor(x => x.ShortName)
            .NotEmpty();

        RuleFor(x => x.SearchTerms)
            .NotEmpty();

        RuleForEach(x => x.SearchTerms)
            .NotEmpty();
    }
}