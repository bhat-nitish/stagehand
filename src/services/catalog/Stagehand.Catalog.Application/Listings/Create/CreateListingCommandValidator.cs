using FluentValidation;

namespace Stagehand.Catalog.Application.Listings.Create;

public sealed class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
    public CreateListingCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(c => c.StartsAt)
            .Must(BeInTheFuture)
            .WithMessage("StartsAt must be in the future.");
    }

    private static bool BeInTheFuture(DateTimeOffset startsAt) =>
        startsAt > DateTimeOffset.UtcNow;
}
