using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;

namespace Stagehand.Catalog.Application.Listings.GetById;

internal sealed class GetListingByIdQueryHandler : IQueryHandler<GetListingByIdQuery, Result<ListingResponse>>
{
    private readonly IListingRepository _listingRepository;

    public GetListingByIdQueryHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Result<ListingResponse>> Handle(
        GetListingByIdQuery query,
        CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(query.ListingId, cancellationToken);

        if (listing is null)
        {
            return Result.Failure<ListingResponse>(ListingErrors.NotFound(query.ListingId));
        }

        return new ListingResponse(
            listing.Id.Value,
            listing.Title,
            listing.Description,
            listing.StartsAt,
            listing.Status.ToString());
    }
}
