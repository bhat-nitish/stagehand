using System.Buffers.Text;
using System.Globalization;
using System.Text;
using Stagehand.Catalog.Domain.Listings;

namespace Stagehand.Catalog.Application.Listings.Search;

public sealed record ListingsCursor(DateTimeOffset StartsAt, ListingId Id)
{
    public string Encode()
    {
        var raw = $"{StartsAt.UtcDateTime.ToString("O", CultureInfo.InvariantCulture)}|{Id.Value}";
        return Base64Url.EncodeToString(Encoding.UTF8.GetBytes(raw));
    }

    public static ListingsCursor? Decode(string? cursor)
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

            var startsAt = DateTimeOffset.Parse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var id = new ListingId(Guid.Parse(parts[1]));

            return new ListingsCursor(startsAt, id);
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
