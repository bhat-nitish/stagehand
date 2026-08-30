using Stagehand.Inventory.Domain.StockItems;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.Release;

public sealed record ReleaseStockCommand(StockItemId StockItemId, Guid ReservationId) : ICommand<Result>;
