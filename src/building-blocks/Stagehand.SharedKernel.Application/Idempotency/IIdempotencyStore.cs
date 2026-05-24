namespace Stagehand.SharedKernel.Application.Idempotency;

public sealed record IdempotentRecord(string RequestHash, int? StatusCode, string? ResponseBody);

public interface IIdempotencyStore
{
    Task<IdempotentRecord?> GetAsync(string key, CancellationToken cancellationToken);

    Task<bool> TryReserveAsync(string key, string requestHash, CancellationToken cancellationToken);

    Task CompleteAsync(string key, int statusCode, string responseBody, CancellationToken cancellationToken);
}
