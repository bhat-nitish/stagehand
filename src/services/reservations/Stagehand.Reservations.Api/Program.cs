using Asp.Versioning;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Stagehand.Reservations.Api;
using Stagehand.Reservations.Api.Infrastructure;
using Stagehand.Reservations.Application;
using Stagehand.Reservations.Infrastructure;
using Stagehand.Contracts.Inventory;
using Stagehand.Contracts.Reservations;
using Stagehand.Reservations.Infrastructure.Messaging.Sagas;
using Stagehand.Reservations.Infrastructure.Persistence;
using Stagehand.ServiceDefaults;
using Stagehand.SharedKernel.Bus;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReservationsApplication(builder.Configuration);
builder.Services.AddReservationsInfrastructure(builder.Configuration);
// Commands are Sent to a specific queue, not published. Queue names come from
// SetKebabCaseEndpointNameFormatter applied to the consumer name.
EndpointConvention.Map<ReserveStock>(new Uri("queue:reserve-stock"));
EndpointConvention.Map<ReleaseStock>(new Uri("queue:release-stock"));
EndpointConvention.Map<ConfirmReservation>(new Uri("queue:confirm-reservation"));
EndpointConvention.Map<RejectReservation>(new Uri("queue:reject-reservation"));
EndpointConvention.Map<ExpireReservation>(new Uri("queue:expire-reservation"));

builder.Services.AddStagehandMessaging(builder.Configuration, messaging =>
{
    messaging.AddConsumers(typeof(ReservationsDbContext).Assembly);

    messaging.AddSagaStateMachine<ReservationStateMachine, ReservationState>()
        .EntityFrameworkRepository(repository =>
        {
            repository.ExistingDbContext<ReservationsDbContext>();
            repository.UsePostgres();
        });

    messaging.AddEntityFrameworkOutbox<ReservationsDbContext>(outbox =>
    {
        outbox.UsePostgres();
        outbox.UseBusOutbox();
    });

    messaging.AddConfigureEndpointsCallback((context, _, cfg) =>
        cfg.UseEntityFrameworkOutbox<ReservationsDbContext>(context));
});

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata =
            builder.Configuration.GetValue("Keycloak:RequireHttpsMetadata", true);   // secure by default
        options.TokenValidationParameters.ValidIssuers =
            builder.Configuration.GetSection("Keycloak:ValidIssuers").Get<string[]>();
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorizationBuilder()
    .AddScopePolicy("reservations:read")
    .AddScopePolicy("reservations:write");

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = ApiVersions.V1;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    });

builder.Services.AddEndpointModules(typeof(Program).Assembly);

var app = builder.Build();

async Task ApplyMigrationsAsync()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ReservationsDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (args.Length > 0 && args[0] == "migrate")
{
    await ApplyMigrationsAsync();
    return;
}

if (app.Environment.IsDevelopment())
{
    await ApplyMigrationsAsync();
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapEndpointModules();

app.Run();
