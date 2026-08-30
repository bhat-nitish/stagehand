using Stagehand.Inventory.Application.Abstractions.Persistence;
using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Inventory.Application.StockItems.Create;

internal sealed class CreateStockItemCommandHandler
    : ICommandHandler<CreateStockItemCommand, Result<StockItemId>>
{
    private readonly IStockItemRepository _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStockItemCommandHandler(IStockItemRepository stockItemRepository, IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StockItemId>> Handle(CreateStockItemCommand command, CancellationToken cancellationToken)
    {
        var existing = await _stockItemRepository.GetByListingAsync(command.ListingId, cancellationToken);

        if (existing is not null)
        {
            return Result.Failure<StockItemId>(StockItemErrors.AlreadyExistsForListing(command.ListingId));
        }

        var stockItem = StockItem.Create(command.ListingId, command.TotalQuantity);

        _stockItemRepository.Add(stockItem);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return stockItem.Id;
    }
}
