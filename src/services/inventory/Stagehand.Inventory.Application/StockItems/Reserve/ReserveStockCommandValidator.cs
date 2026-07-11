using FluentValidation;

namespace Stagehand.Inventory.Application.StockItems.Reserve;

public sealed class ReserveStockCommandValidator : AbstractValidator<ReserveStockCommand>
{
    public ReserveStockCommandValidator()
    {
        RuleFor(c => c.Quantity)
            .GreaterThan(0);
    }
}
