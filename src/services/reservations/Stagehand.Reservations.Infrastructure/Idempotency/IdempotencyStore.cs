using Microsoft.EntityFrameworkCore;
using Npgsql;
using Stagehand.Reservations.Infrastructure.Persistence;
using Stagehand.SharedKernel.Application.Idempotency;

namespace Stagehand.Reservations.Infrastructure.Idempotency;

internal sealed class IdempotencyStore : IIdempotencyStore
{
    private readonly ReservationsDbContext _dbContext;

    public IdempotencyStore(ReservationsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IdempotentRecord?> GetAsync(string key, CancellationToken cancellationToken)
    {
        var row = await _dbContext.Set<IdempotentRequest>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Key == key, cancellationToken);

        return row is null
            ? null
            : new IdempotentRecord(row.RequestHash, row.StatusCode, row.ResponseBody);
    }

    public async Task<bool> TryReserveAsync(string key, string requestHash, CancellationToken cancellationToken)
    {
        _dbContext.Set<IdempotentRequest>().Add(new IdempotentRequest
        {
            Key = key,
            RequestHash = requestHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
        });

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            _dbContext.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task CompleteAsync(string key, int statusCode, string responseBody, CancellationToken cancellationToken)
    {
        var row = await _dbContext.Set<IdempotentRequest>()
            .FirstAsync(r => r.Key == key, cancellationToken);

        row.StatusCode = statusCode;
        row.ResponseBody = responseBody;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
