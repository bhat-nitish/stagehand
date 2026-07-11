using Asp.Versioning.Builder;
using MediatR;
using Stagehand.Inventory.Api.Infrastructure;
using Stagehand.Inventory.Application.StockItems.Close;
using Stagehand.Inventory.Application.StockItems.Create;
using Stagehand.Inventory.Application.StockItems.GetById;
using Stagehand.Inventory.Application.StockItems.Release;
using Stagehand.Inventory.Application.StockItems.Reserve;
using Stagehand.Inventory.Application.StockItems.Search;
using Stagehand.Inventory.Domain.StockItems;
using Stagehand.ServiceDefaults;

namespace Stagehand.Inventory.Api.Endpoints;

internal sealed class StockItemsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versions = app.NewApiVersionSet()
            .HasApiVersion(ApiVersions.V1)
            .HasApiVersion(ApiVersions.V2)
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/stock-items")
            .WithApiVersionSet(versions)
            .WithTags("StockItems").RequireAuthorization();

        group.MapPost("/", CreateAsync)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .RequireAuthorization("inventory:write");

        group.MapGet("/", SearchAsync)
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization("inventory:read");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .HasApiVersion(ApiVersions.V1)
            .WithName(nameof(GetByIdAsync))
            .RequireAuthorization("inventory:read");

        group.MapPost("/{id:guid}/reserve", ReserveAsync)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .RequireAuthorization("inventory:write");

        group.MapPost("/{id:guid}/release", ReleaseAsync)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .RequireAuthorization("inventory:write");

        group.MapPost("/{id:guid}/close", CloseAsync)
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization("inventory:write");

        group.MapGet("/ping", Ping)
            .HasApiVersion(ApiVersions.V2).AllowAnonymous();
    }

    private static IResult Ping() =>
        Results.Ok(new { version = "2.0", message = "Inventory v2 is alive." });

    private static async Task<IResult> CreateAsync(
        CreateStockItemRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateStockItemCommand(request.ListingId, request.TotalQuantity);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.ToProblem();
        }

        var id = result.Value.Value;

        return TypedResults.CreatedAtRoute(
            routeName: nameof(GetByIdAsync),
            routeValues: new { id, version = ApiVersions.V1Route },
            value: new { id });
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetStockItemByIdQuery(new StockItemId(id));
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    private static async Task<IResult> SearchAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        string? cursor = null,
        int pageSize = 20,
        StockItemStatus? status = null)
    {
        var query = new SearchStockItemsQuery(cursor, pageSize, status);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    // Request DTO first so the IdempotencyEndpointFilter hashes the body (Quantity),
    // not the route id. The {id} route value still binds by name.
    private static async Task<IResult> ReserveAsync(
        ReserveStockRequest request,
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReserveStockCommand(new StockItemId(id), request.Quantity);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.NoContent();
    }

    private static async Task<IResult> ReleaseAsync(
        ReleaseStockRequest request,
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReleaseStockCommand(new StockItemId(id), request.Quantity);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.NoContent();
    }

    private static async Task<IResult> CloseAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CloseStockItemCommand(new StockItemId(id));
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.NoContent();
    }
}
