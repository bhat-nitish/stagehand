using Microsoft.AspNetCore.Authentication.JwtBearer;
using Stagehand.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.RequireHttpsMetadata =
            builder.Configuration.GetValue("Keycloak:RequireHttpsMetadata", true);
        options.TokenValidationParameters.ValidIssuers =
            builder.Configuration.GetSection("Keycloak:ValidIssuers").Get<string[]>();
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();