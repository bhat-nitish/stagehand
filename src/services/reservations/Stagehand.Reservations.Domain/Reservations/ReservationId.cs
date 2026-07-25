namespace Stagehand.Reservations.Domain.Reservations;

public readonly record struct ReservationId(Guid Value) : IComparable<ReservationId>
{
    public static ReservationId New() => new(Guid.NewGuid());

    public int CompareTo(ReservationId other) => Value.CompareTo(other.Value);

    public static bool operator <(ReservationId left, ReservationId right) => left.CompareTo(right) < 0;

    public static bool operator >(ReservationId left, ReservationId right) => left.CompareTo(right) > 0;

    public static bool operator <=(ReservationId left, ReservationId right) => left.CompareTo(right) <= 0;

    public static bool operator >=(ReservationId left, ReservationId right) => left.CompareTo(right) >= 0;
}
