using FluentValidation;
using JobScraper.Contracts.Requests.Websites;

namespace JobScraper.Application.Features.WebsiteManagement.Validation;

public class UpdateWebsiteRequestValidator : AbstractValidator<UpdateWebsiteRequest>
{
    public UpdateWebsiteRequestValidator()
    {
        // At least one property must be provided for update
        RuleFor(x => x)
            .Must(x => x.Url != null || x.ShortName != null || x.SearchTerms != null)
            .WithMessage("At least one property (Url, ShortName, or SearchTerms) must be provided for update");

        // Basic URL validation if provided
        When(x => x.Url != null, () =>
        {
            RuleFor(x => x.Url)
                .NotEmpty();
        });

        // Basic ShortName validation if provided
        When(x => x.ShortName != null, () =>
        {
            RuleFor(x => x.ShortName)
                .NotEmpty();
        });

        // Basic SearchTerms validation if provided
        When(x => x.SearchTerms != null, () =>
        {
            RuleFor(x => x.SearchTerms)
                .NotEmpty()
                .WithMessage("SearchTerms list cannot be empty when provided");

            RuleForEach(x => x.SearchTerms)
                .NotEmpty();
        });
    }
}