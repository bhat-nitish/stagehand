using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Inventory.Application.StockItems.Reserve;

internal sealed class ReserveStockCommandHandler : ICommandHandler<ReserveStockCommand, Result>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockCommandHandler(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReserveStockCommand command, CancellationToken cancellationToken)
    {
        var stockItem = await _stockItemRepository.GetByIdAsync(command.StockItemId, cancellationToken);

        if (stockItem is null)
        {
            return Result.Failure(StockItemErrors.NotFound(command.StockItemId));
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
            return Result.Failure(StockItemErrors.ConcurrencyConflict(command.StockItemId));
        }

        return Result.Success();
    }
}
