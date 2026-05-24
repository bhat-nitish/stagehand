namespace Stagehand.Catalog.Infrastructure.Idempotency;

internal sealed class IdempotentRequest
{
    public string Key { get; set; } = default!;

    public string RequestHash { get; set; } = default!;

    public int? StatusCode { get; set; }

    public string? ResponseBody { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
