using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Application;
using Stagehand.Reservations.Infrastructure.Messaging.Sagas;
using Stagehand.Reservations.Infrastructure.Persistence;

namespace Stagehand.Reservations.Infrastructure.Messaging;

internal sealed partial class ReservationHoldSweeper : BackgroundService
{
    private const int BatchSize = 100;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ReservationHoldSweeper> _logger;
    private readonly ReservationOptions _options;

    public ReservationHoldSweeper(
        IServiceScopeFactory scopeFactory,
        TimeProvider timeProvider,
        ILogger<ReservationHoldSweeper> logger,
        IOptions<ReservationOptions> options)
    {
        _scopeFactory = scopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(_options.SweepIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SweepAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogSweepFailed(exception);
            }

            await Task.Delay(interval, _timeProvider, stoppingToken);
        }
    }

    private async Task SweepAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

        var now = _timeProvider.GetUtcNow();

        var elapsed = await dbContext.Set<ReservationState>()
            .Where(state => state.CurrentState == nameof(ReservationStateMachine.AwaitingStock)
                && state.ExpiresAt <= now)
            .Select(state => state.CorrelationId)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (elapsed.Count == 0)
        {
            return;
        }

        foreach (var reservationId in elapsed)
        {
            await publishEndpoint.Publish(new ReservationHoldElapsed(reservationId), cancellationToken);
        }

        // The bus outbox stages publishes on this DbContext; without SaveChanges they never send.
        await dbContext.SaveChangesAsync(cancellationToken);

        LogSwept(elapsed.Count);
    }

    [LoggerMessage(LogLevel.Error, "Reservation hold sweep failed")]
    private partial void LogSweepFailed(Exception exception);

    [LoggerMessage(LogLevel.Information, "Swept {Count} elapsed reservation hold(s)")]
    private partial void LogSwept(int count);
}
