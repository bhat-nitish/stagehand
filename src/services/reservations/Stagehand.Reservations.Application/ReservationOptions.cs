namespace Stagehand.Reservations.Application;

public sealed class ReservationOptions
{
    public const string SectionName = "Reservations";

    public int HoldMinutes { get; set; } = 10;

    public int SweepIntervalSeconds { get; set; } = 30;
}
