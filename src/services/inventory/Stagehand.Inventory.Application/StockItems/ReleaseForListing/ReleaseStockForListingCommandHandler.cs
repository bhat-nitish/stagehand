using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Inventory.Application.StockItems.ReleaseForListing;

internal sealed class ReleaseStockForListingCommandHandler
    : ICommandHandler<ReleaseStockForListingCommand, Result>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseStockForListingCommandHandler(
        IStockItemRepository stockItemRepository,
        IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReleaseStockForListingCommand command,
        CancellationToken cancellationToken)
    {
        var stockItem = await _stockItemRepository.GetByListingAsync(command.ListingId, cancellationToken);

        // No stock item means nothing was ever held for this listing.
        if (stockItem is null)
        {
            return Result.Success();
        }

        var result = stockItem.Release(command.ReservationId);

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
