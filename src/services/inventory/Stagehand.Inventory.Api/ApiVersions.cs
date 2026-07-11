using Asp.Versioning;

namespace Stagehand.Inventory.Api;

internal static class ApiVersions
{
    public static readonly ApiVersion V1 = new(1, 0);
    public static readonly ApiVersion V2 = new(2, 0);

    public const string V1Route = "1";
    public const string V2Route = "2";
}
