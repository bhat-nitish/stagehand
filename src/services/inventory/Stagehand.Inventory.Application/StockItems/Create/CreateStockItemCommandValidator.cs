using FluentValidation;

namespace Stagehand.Inventory.Application.StockItems.Create;

public sealed class CreateStockItemCommandValidator : AbstractValidator<CreateStockItemCommand>
{
    public CreateStockItemCommandValidator()
    {
        RuleFor(c => c.ListingId)
            .NotEmpty();

        RuleFor(c => c.TotalQuantity)
            .GreaterThan(0);
    }
}
