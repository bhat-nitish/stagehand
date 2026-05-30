using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Stagehand.Catalog.Api;
using Stagehand.Catalog.Api.Infrastructure;
using Stagehand.Catalog.Application;
using Stagehand.Catalog.Infrastructure;
using Stagehand.Catalog.Infrastructure.Persistence;
using Stagehand.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

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

app.MapDefaultEndpoints();
app.MapEndpointModules();

app.Run();
