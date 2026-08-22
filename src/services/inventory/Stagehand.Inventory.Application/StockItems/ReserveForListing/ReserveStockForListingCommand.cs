using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.ReserveForListing;

public sealed record ReserveStockForListingCommand(Guid ReservationId, Guid ListingId, int Quantity)
    : ICommand<Result>;
