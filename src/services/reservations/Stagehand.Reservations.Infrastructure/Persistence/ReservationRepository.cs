using Microsoft.EntityFrameworkCore;
using Stagehand.Reservations.Application.Abstractions.Persistence;
using Stagehand.Reservations.Application.Reservations.Search;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Infrastructure.Persistence;

internal sealed class ReservationRepository : IReservationRepository
{
    private readonly ReservationsDbContext _dbContext;

    public ReservationRepository(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken) =>
        _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Reservation>> SearchAsync(
        ReservationsCursor? cursor,
        int limit,
        ReservationStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Reservations.AsNoTracking();

        if (status is not null)
        {
            var statusValue = status.Value;
            query = query.Where(r => r.Status == statusValue);
        }

        if (cursor is not null)
        {
            var createdAt = cursor.CreatedAt;
            var id = cursor.Id;
            query = query.Where(r => r.CreatedAt > createdAt || (r.CreatedAt == createdAt && r.Id > id));
        }

        return await query
            .OrderBy(r => r.CreatedAt)
            .ThenBy(r => r.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void Add(Reservation reservation) =>
        _dbContext.Reservations.Add(reservation);
}
