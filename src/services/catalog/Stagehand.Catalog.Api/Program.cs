using Microsoft.EntityFrameworkCore;
using Stagehand.Catalog.Api.Endpoints;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseExceptionHandler();

app.MapDefaultEndpoints();
app.MapListingEndpoints();

app.Run();
