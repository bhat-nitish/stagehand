using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Stagehand.Catalog.Api;
using Stagehand.Catalog.Api.Infrastructure;
using Stagehand.Catalog.Application;
using Stagehand.Catalog.Infrastructure;
using Stagehand.Catalog.Infrastructure.Persistence;
using Stagehand.ServiceDefaults;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);

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
    .AddScopePolicy("catalog:read")
    .AddScopePolicy("catalog:write");

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
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
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
