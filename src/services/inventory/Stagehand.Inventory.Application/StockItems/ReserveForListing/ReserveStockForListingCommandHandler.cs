using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Inventory.Application.StockItems.ReserveForListing;

internal sealed class ReserveStockForListingCommandHandler
    : ICommandHandler<ReserveStockForListingCommand, Result>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockForListingCommandHandler(
        IStockItemRepository stockItemRepository,
        IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReserveStockForListingCommand command,
        CancellationToken cancellationToken)
    {
        var stockItem = await _stockItemRepository.GetByListingAsync(command.ListingId, cancellationToken);

        if (stockItem is null)
        {
            return Result.Failure(StockItemErrors.NoStockItemForListing(command.ListingId));
        }

        var result = stockItem.Reserve(command.ReservationId, command.Quantity);

        if (result.IsFailure)
        {
            return result;
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(StockItemErrors.ConcurrencyConflict(stockItem.Id));
        }

        return Result.Success();
    }
}
