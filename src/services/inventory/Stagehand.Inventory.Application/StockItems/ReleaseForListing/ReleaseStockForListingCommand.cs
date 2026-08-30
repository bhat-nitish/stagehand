using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Inventory.Application.StockItems.ReleaseForListing;

public sealed record ReleaseStockForListingCommand(Guid ReservationId, Guid ListingId) : ICommand<Result>;
