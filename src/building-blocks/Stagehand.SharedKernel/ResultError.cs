namespace Stagehand.SharedKernel;

public sealed record ResultError(string Code, string Description, ErrorType Type)
{
    public static readonly ResultError None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly ResultError NullValue = new("ResultError.NullValue", "The specified result value is null.", ErrorType.Failure);
}
