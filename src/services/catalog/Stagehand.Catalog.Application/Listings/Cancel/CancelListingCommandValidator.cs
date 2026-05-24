using FluentValidation;

namespace Stagehand.Catalog.Application.Listings.Cancel;

public sealed class CancelListingCommandValidator : AbstractValidator<CancelListingCommand>
{
    public CancelListingCommandValidator()
    {
        RuleFor(c => c.Reason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
