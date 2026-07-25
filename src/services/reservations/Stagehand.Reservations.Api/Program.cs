using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Stagehand.Reservations.Api;
using Stagehand.Reservations.Api.Infrastructure;
using Stagehand.Reservations.Application;
using Stagehand.Reservations.Infrastructure;
using Stagehand.Reservations.Infrastructure.Persistence;
using Stagehand.ServiceDefaults;
using Stagehand.SharedKernel.Bus;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddReservationsApplication(builder.Configuration);
builder.Services.AddReservationsInfrastructure(builder.Configuration);
builder.Services.AddStagehandMessaging(builder.Configuration);

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
