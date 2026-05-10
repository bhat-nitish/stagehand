using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Stagehand.SharedKernel.Application.Behaviours;

public sealed partial class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        LogHandling(requestName);

        try
        {
            var response = await next();

            stopwatch.Stop();
            LogHandled(requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogHandlerError(ex, requestName, stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    [LoggerMessage(LogLevel.Information, "Handling {RequestName}")]
    private partial void LogHandling(string requestName);

    [LoggerMessage(LogLevel.Information, "Handled {RequestName} in {ElapsedMs} ms")]
    private partial void LogHandled(string requestName, long elapsedMs);

    [LoggerMessage(LogLevel.Error, "Error handling {RequestName} after {ElapsedMs} ms")]
    private partial void LogHandlerError(Exception ex, string requestName, long elapsedMs);
}
