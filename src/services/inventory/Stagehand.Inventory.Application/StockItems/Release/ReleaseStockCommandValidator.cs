using FluentValidation;

namespace Stagehand.Inventory.Application.StockItems.Release;

public sealed class ReleaseStockCommandValidator : AbstractValidator<ReleaseStockCommand>
{
    public ReleaseStockCommandValidator()
    {
        RuleFor(c => c.ReservationId)
            .NotEmpty();
    }
}
