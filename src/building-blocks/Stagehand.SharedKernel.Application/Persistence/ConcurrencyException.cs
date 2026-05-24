namespace Stagehand.SharedKernel.Application.Persistence;

public sealed class ConcurrencyException : Exception
{
    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
