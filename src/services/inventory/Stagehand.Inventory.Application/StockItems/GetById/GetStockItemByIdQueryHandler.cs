using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.GetById;

internal sealed class GetStockItemByIdQueryHandler
    : IQueryHandler<GetStockItemByIdQuery, Result<StockItemResponse>>
{
    private readonly IStockItemRepository _stockItemRepository;

    public GetStockItemByIdQueryHandler(IStockItemRepository stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task<Result<StockItemResponse>> Handle(
        GetStockItemByIdQuery query,
        CancellationToken cancellationToken)
    {
        var stockItem = await _stockItemRepository.GetByIdAsync(query.StockItemId, cancellationToken);

        if (stockItem is null)
        {
            return Result.Failure<StockItemResponse>(StockItemErrors.NotFound(query.StockItemId));
        }

        return new StockItemResponse(
            stockItem.Id.Value,
            stockItem.ListingId,
            stockItem.TotalQuantity,
            stockItem.ReservedQuantity,
            stockItem.AvailableQuantity,
            stockItem.Status.ToString());
    }
}
