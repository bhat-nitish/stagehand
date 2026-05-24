namespace Stagehand.Catalog.Domain.Listings;

public readonly record struct ListingId(Guid Value) : IComparable<ListingId>
{
    public static ListingId New() => new(Guid.NewGuid());

    public int CompareTo(ListingId other) => Value.CompareTo(other.Value);

    public static bool operator <(ListingId left, ListingId right) => left.CompareTo(right) < 0;

    public static bool operator >(ListingId left, ListingId right) => left.CompareTo(right) > 0;

    public static bool operator <=(ListingId left, ListingId right) => left.CompareTo(right) <= 0;

    public static bool operator >=(ListingId left, ListingId right) => left.CompareTo(right) >= 0;
}
