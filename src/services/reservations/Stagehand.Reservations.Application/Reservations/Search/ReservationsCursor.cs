using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Stagehand.Reservations.Domain.Reservations;

namespace Stagehand.Reservations.Application.Reservations.Search;

public sealed record ReservationsCursor(DateTimeOffset CreatedAt, ReservationId Id)
{
    public string Encode()
    {
        var raw = $"{CreatedAt.UtcDateTime.ToString("O", CultureInfo.InvariantCulture)}|{Id.Value}";
        return Base64Url.EncodeToString(Encoding.UTF8.GetBytes(raw));
    }

    public static ReservationsCursor? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }

        try
        {
            var raw = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor));
            var parts = raw.Split('|');

            if (parts.Length != 2)
            {
                return null;
            }

            var createdAt = DateTimeOffset.Parse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var id = new ReservationId(Guid.Parse(parts[1]));

            return new ReservationsCursor(createdAt, id);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
