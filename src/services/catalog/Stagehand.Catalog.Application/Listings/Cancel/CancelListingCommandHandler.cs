using Stagehand.Catalog.Application.Abstractions.Persistence;
using Stagehand.SharedKernel;
using Stagehand.SharedKernel.Application.Messaging;
using Stagehand.SharedKernel.Application.Persistence;

namespace Stagehand.Catalog.Application.Listings.Cancel;

internal sealed class CancelListingCommandHandler : ICommandHandler<CancelListingCommand, Result>
{
    private readonly IListingRepository _listingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelListingCommandHandler(IListingRepository listingRepository, IUnitOfWork unitOfWork)
    {
        _listingRepository = listingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelListingCommand command, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(command.ListingId, cancellationToken);

        if (listing is null)
        {
            return Result.Failure(ListingErrors.NotFound(command.ListingId));
        }

        listing.Cancel(command.Reason);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure(ListingErrors.ConcurrencyConflict(command.ListingId));
        }

        return Result.Success();
    }
}
