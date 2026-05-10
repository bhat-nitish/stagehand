using MediatR;
using Stagehand.Catalog.Application.Listings.Create;
using Stagehand.Catalog.Application.Listings.GetById;
using Stagehand.Catalog.Domain.Listings;

namespace Stagehand.Catalog.Api.Endpoints;

internal static class ListingEndpoints
{
    public static IEndpointRouteBuilder MapListingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/listings").WithTags("Listings");

        group.MapPost("/", CreateAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);

        return app;
    }

    private static async Task<IResult> CreateAsync(
        CreateListingCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var id = result.Value.Value;
        return Results.Created($"/listings/{id}", new { id });
    }

    private static async Task<IResult> GetByIdAsync(
        Guid id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetListingByIdQuery(new ListingId(id));
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return Results.Problem(
                title: result.Error.Code,
                detail: result.Error.Description,
                statusCode: StatusCodes.Status404NotFound);
        }

        return Results.Ok(result.Value);
    }
}
