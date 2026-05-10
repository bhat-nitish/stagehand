using Stagehand.Catalog.Domain.Listings.Events;
using Stagehand.SharedKernel;

namespace Stagehand.Catalog.Domain.Listings;

public sealed class Listing : AggregateRoot<ListingId>
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public ListingStatus Status { get; private set; }

    private Listing(ListingId id, string title, string description, DateTimeOffset startsAt)
        : base(id)
    {
        Title = title;
        Description = description;
        StartsAt = startsAt;
        Status = ListingStatus.Scheduled;
    }

    public static Listing Create(string title, string description, DateTimeOffset startsAt)
    {
        var listing = new Listing(ListingId.New(), title, description, startsAt);
        listing.RaiseDomainEvent(new ListingCreated(listing.Id, title, startsAt));
        return listing;
    }

    public void Cancel(string reason)
    {
        if (Status == ListingStatus.Cancelled)
        {
             return;
        }
        Status = ListingStatus.Cancelled;
        RaiseDomainEvent(new ListingCancelled(Id, reason));
    }
}