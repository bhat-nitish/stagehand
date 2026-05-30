using Asp.Versioning.Builder;
using MediatR;
using Stagehand.Catalog.Api.Infrastructure;
using Stagehand.Catalog.Application.Listings.Cancel;
using Stagehand.Catalog.Application.Listings.Create;
using Stagehand.Catalog.Application.Listings.GetById;
using Stagehand.Catalog.Application.Listings.Search;
using Stagehand.Catalog.Domain.Listings;
using Stagehand.ServiceDefaults;

namespace Stagehand.Catalog.Api.Endpoints;

internal sealed class ListingsModule : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var versions = app.NewApiVersionSet()
            .HasApiVersion(ApiVersions.V1)
            .HasApiVersion(ApiVersions.V2)
            .ReportApiVersions()
            .Build();

        var group = app.MapGroup("/v{version:apiVersion}/listings")
            .WithApiVersionSet(versions)
            .WithTags("Listings");

        group.MapPost("/", CreateAsync)
            .HasApiVersion(ApiVersions.V1)
            .AddEndpointFilter<IdempotencyEndpointFilter>();

        group.MapGet("/", SearchAsync)
            .HasApiVersion(ApiVersions.V1);

        group.MapGet("/{id:guid}", GetByIdAsync)
            .HasApiVersion(ApiVersions.V1)
            .WithName(nameof(GetByIdAsync));

        group.MapPost("/{id:guid}/cancel", CancelAsync)
            .HasApiVersion(ApiVersions.V1);

        group.MapGet("/ping", Ping)
            .HasApiVersion(ApiVersions.V2);
    }

    private static IResult Ping() =>
        Results.Ok(new { version = "2.0", message = "Catalog v2 is alive." });

    private static async Task<IResult> CreateAsync(
        CreateListingRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateListingCommand(request.Title, request.Description, request.StartsAt);
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
        var query = new GetListingByIdQuery(new ListingId(id));
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    private static async Task<IResult> SearchAsync(
        IMediator mediator,
        CancellationToken cancellationToken,
        string? cursor = null,
        int pageSize = 20,
        ListingStatus? status = null)
    {
        var query = new SearchListingsQuery(cursor, pageSize, status);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.Ok(result.Value);
    }

    private static async Task<IResult> CancelAsync(
        Guid id,
        CancelListingRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CancelListingCommand(new ListingId(id), request.Reason);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure ? result.ToProblem() : Results.NoContent();
    }
}
