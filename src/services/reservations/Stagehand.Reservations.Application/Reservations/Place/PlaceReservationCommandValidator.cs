using FluentValidation;

namespace Stagehand.Reservations.Application.Reservations.Place;

public sealed class PlaceReservationCommandValidator : AbstractValidator<PlaceReservationCommand>
{
    public PlaceReservationCommandValidator()
    {
        RuleFor(c => c.ListingId)
            .NotEmpty();

        RuleFor(c => c.Quantity)
            .GreaterThan(0);
    }
}
