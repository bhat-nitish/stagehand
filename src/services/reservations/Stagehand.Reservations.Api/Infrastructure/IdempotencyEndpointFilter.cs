using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Stagehand.SharedKernel.Application.Idempotency;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Reservations.Api.Infrastructure;

internal sealed class IdempotencyEndpointFilter : IEndpointFilter
{
    private const string HeaderName = "Idempotency-Key";

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var http = context.HttpContext;

        if (!http.Request.Headers.TryGetValue(HeaderName, out var headerValues)
            || string.IsNullOrWhiteSpace(headerValues.ToString()))
        {
            return Results.Problem(
                title: "The Idempotency-Key header is required.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var key = headerValues.ToString();
        var requestHash = ComputeRequestHash(http, context);
        var cancellationToken = http.RequestAborted;

        var services = http.RequestServices;
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        var store = services.GetRequiredService<IIdempotencyStore>();

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var reserved = await store.TryReserveAsync(key, requestHash, cancellationToken);
        if (!reserved)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return await ReplayAsync(store, key, requestHash, cancellationToken);
        }

        object? result;
        try
        {
            result = await next(context);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        var (statusCode, body) = await CaptureAsync(http, result);

        if (statusCode is >= 200 and < 300)
        {
            await store.CompleteAsync(key, statusCode, body, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        else
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
        }

        if (!string.IsNullOrEmpty(body))
        {
            await http.Response.WriteAsync(body, cancellationToken);
        }

        return Results.Empty;
    }

    private static async ValueTask<IResult> ReplayAsync(
        IIdempotencyStore store,
        string key,
        string requestHash,
        CancellationToken cancellationToken)
    {
        var existing = await store.GetAsync(key, cancellationToken);

        if (existing is null)
        {
            return Results.Problem(
                title: "A request with this Idempotency-Key is already in progress.",
                statusCode: StatusCodes.Status409Conflict);
        }

        if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
        {
            return Results.Problem(
                title: "The Idempotency-Key was reused with different request parameters.",
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Results.Content(
            existing.ResponseBody ?? string.Empty,
            "application/json",
            statusCode: existing.StatusCode);
    }

    private static string ComputeRequestHash(HttpContext http, EndpointFilterInvocationContext context)
    {
        var payload = context.Arguments.Count > 0 ? context.Arguments[0] : null;
        var payloadJson = payload is null
            ? "null"
            : JsonSerializer.Serialize(payload, payload.GetType());

        var raw = $"{http.Request.Method}:{http.Request.Path}:{payloadJson}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }

    private static async Task<(int StatusCode, string Body)> CaptureAsync(HttpContext http, object? result)
    {
        var originalBody = http.Response.Body;
        await using var buffer = new MemoryStream();
        http.Response.Body = buffer;

        try
        {
            if (result is IResult endpointResult)
            {
                await endpointResult.ExecuteAsync(http);
            }
        }
        finally
        {
            http.Response.Body = originalBody;
        }

        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        return (http.Response.StatusCode, body);
    }
}
