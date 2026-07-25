using Asp.Versioning.Builder;
using MediatR;
using Stagehand.Reservations.Api.Infrastructure;
using Stagehand.Reservations.Application.Reservations.Cancel;
using Stagehand.Reservations.Application.Reservations.GetById;
using Stagehand.Reservations.Application.Reservations.Place;
using Stagehand.Reservations.Application.Reservations.Search;
using Stagehand.Reservations.Domain.Reservations;
using Stagehand.ServiceDefaults;

namespace Stagehand.Reservations.Api.Endpoints;

internal sealed class ReservationsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versions = app.NewApiVersionSet()
            .HasApiVersion(ApiVersions.V1)
            .HasApiVersion(ApiVersions.V2)
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/reservations")
            .WithApiVersionSet(versions)
            .WithTags("Reservations").RequireAuthorization();

        group.MapPost("/", PlaceAsync)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .RequireAuthorization("reservations:write");

        group.MapGet("/", SearchAsync)
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization("reservations:read");

        group.MapGet("/{id:guid}", GetByIdAsync)
            .HasApiVersion(ApiVersions.V1)
            .WithName(nameof(GetByIdAsync))
            .RequireAuthorization("reservations:read");

        // Not idempotency-protected: a bodiless state transition, same call as Inventory's close.
        group.MapPost("/{id:guid}/cancel", CancelAsync)
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization("reservations:write");

        group.MapGet("/ping", Ping)
            .HasApiVersion(ApiVersions.V2).AllowAnonymous();
    }

    private static IResult Ping() =>
        Results.Ok(new { version = "2.0", message = "Reservations v2 is alive." });

    private static async Task<IResult> PlaceAsync(
        PlaceReservationRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new PlaceReservationCommand(request.ListingId, request.Quantity);
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
        var query = new GetReservationByIdQuery(new ReservationId(id));
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    private static async Task<IResult> SearchAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        string? cursor = null,
        int pageSize = 20,
        ReservationStatus? status = null)
    {
        var query = new SearchReservationsQuery(cursor, pageSize, status);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    private static async Task<IResult> CancelAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(new ReservationId(id));
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.NoContent();
    }
}
