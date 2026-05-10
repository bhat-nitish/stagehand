using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.Catalog.Domain.Listings;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Catalog.Application.Listings.Create;

internal sealed class CreateListingCommandHandler : ICommandHandler<CreateListingCommand, Result<ListingId>>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ListingId>> Handle(CreateListingCommand command, CancellationToken cancellationToken)
    {
        var listing = Listing.Create(command.Title, command.Description, command.StartsAt);

        _listingRepository.Add(listing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return listing.Id;
    }
}
