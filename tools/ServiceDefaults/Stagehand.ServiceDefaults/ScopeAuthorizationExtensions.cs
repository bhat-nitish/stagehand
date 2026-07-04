using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Stagehand.ServiceDefaults;

public static class ScopeAuthorizationExtensions
{
    // Registers a policy named after the scope, satisfied only if the token's
    // space-separated "scope" claim contains it.
    public static AuthorizationBuilder AddScopePolicy(this AuthorizationBuilder builder, string scope) =>
        builder.AddPolicy(scope, policy => policy.RequireAssertion(context => context.User.HasScope(scope)));

    public static bool HasScope(this ClaimsPrincipal user, string scope) =>
        user.FindFirst("scope")?.Value
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains(scope)
        ?? false;
}
