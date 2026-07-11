using System.Buffers.Text;
using System.Text;
using Stagehand.Inventory.Domain.StockItems;

namespace Stagehand.Inventory.Application.StockItems.Search;

public sealed record StockItemsCursor(StockItemId Id)
{
    public string Encode() =>
        Base64Url.EncodeToString(Encoding.UTF8.GetBytes(Id.Value.ToString()));

    public static StockItemsCursor? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }

        try
        {
            var raw = Encoding.UTF8.GetString(Base64Url.DecodeFromChars(cursor));

            return Guid.TryParse(raw, out var id)
                ? new StockItemsCursor(new StockItemId(id))
                : null;
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
