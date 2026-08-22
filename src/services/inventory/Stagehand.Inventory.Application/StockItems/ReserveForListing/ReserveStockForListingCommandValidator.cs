using FluentValidation;

namespace Stagehand.Inventory.Application.StockItems.ReserveForListing;

public sealed class ReserveStockForListingCommandValidator
    : AbstractValidator<ReserveStockForListingCommand>
{
    public ReserveStockForListingCommandValidator()
    {
        RuleFor(c => c.ReservationId)
            .NotEmpty();

        RuleFor(c => c.ListingId)
            .NotEmpty();

        RuleFor(c => c.Quantity)
            .GreaterThan(0);
    }
}
