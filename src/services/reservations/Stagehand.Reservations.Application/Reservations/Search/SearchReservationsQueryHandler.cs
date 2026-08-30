using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.Reservations.Application.Reservations.GetById;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Reservations.Application.Reservations.Search;

internal sealed class SearchReservationsQueryHandler
    : IQueryHandler<SearchReservationsQuery, Result<SearchReservationsResponse>>
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IReservationRepository _reservationRepository;

    public SearchReservationsQueryHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<SearchReservationsResponse>> Handle(
        SearchReservationsQuery query,
        CancellationToken cancellationToken)
    {
        var pageSize = query.PageSize <= 0
            ? DefaultPageSize
            : Math.Min(query.PageSize, MaxPageSize);

        var cursor = ReservationsCursor.Decode(query.Cursor);

        var reservations = await _reservationRepository.SearchAsync(
            cursor,
            pageSize + 1,
            query.Status,
            cancellationToken);

        var hasMore = reservations.Count > pageSize;

        var page = hasMore
            ? reservations.Take(pageSize).ToList()
            : reservations;

        var items = page
            .Select(reservation => new ReservationResponse(
                reservation.Id.Value,
                reservation.ListingId,
                reservation.Quantity,
                reservation.Status.ToString(),
                reservation.CreatedAt,
                reservation.ExpiresAt,
                reservation.RejectionReason))
            .ToList();

        var nextCursor = hasMore
            ? new ReservationsCursor(page[^1].CreatedAt, page[^1].Id).Encode()
            : null;

        return new SearchReservationsResponse(items, nextCursor, hasMore);
    }
}
