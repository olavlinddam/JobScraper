using FluentValidation;
using JobScraper.Contracts.Requests.Websites;

namespace JobScraper.Application.Features.WebsiteManagement.Validation;

public class UpdateWebsiteRequestValidator : AbstractValidator<UpdateWebsiteRequest>
{
    public UpdateWebsiteRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Url != null || x.ShortName != null || x.SearchTerms != null)
            .WithMessage("At least one property (Url, ShortName, or SearchTerms) must be provided for update");

        When(x => x.Url != null, () =>
        {
            RuleFor(x => x.Url)
                .NotEmpty()
                .MaximumLength(2000);
        });

        When(x => x.ShortName != null, () =>
        {
            RuleFor(x => x.ShortName)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.SearchTerms != null, () =>
        {
            RuleFor(x => x.SearchTerms)
                .NotEmpty()
                .WithMessage("SearchTerms list cannot be empty when provided");

            RuleForEach(x => x.SearchTerms)
                .NotEmpty()
                .MaximumLength(100);
        });
    }
}