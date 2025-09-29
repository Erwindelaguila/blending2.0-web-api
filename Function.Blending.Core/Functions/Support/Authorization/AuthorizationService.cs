using Function.Blending.Core.Functions.Configuration.Options;
using Function.Blending.Core.Shared.Extensions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Function.Blending.Core.Functions.Support.Authorization;

public sealed class AuthorizationService(IOptions<AuthorizationOptions> opts) : IAuthorizationService
{
  private readonly AuthorizationOptions _opts = opts.Value;

  public bool IsAuthorized(ClaimsPrincipal? user, string scope)
  {
    Console.WriteLine($"[DEBUG AUTH] Checking authorization for scope: '{scope}'");
    Console.WriteLine($"[DEBUG AUTH] Available scopes in Allow: [{string.Join(", ", _opts.Allow.Keys)}]");
    
    if (!_opts.Allow.TryGetValue(scope, out var allowed) || allowed is null || allowed.Length == 0)
    {
      Console.WriteLine($"[DEBUG AUTH] Scope '{scope}' not found in Allow dictionary or has no groups");
      return false;
    }

    Console.WriteLine($"[DEBUG AUTH] Allowed groups for '{scope}': [{string.Join(", ", allowed)}]");

    // Si no hay principal, solo pasa si DevBypass está activo
    if (user is null || !(user.Identity?.IsAuthenticated ?? false))
    {
      Console.WriteLine($"[DEBUG AUTH] No authenticated user, DevBypass: {_opts.DevBypass}");
      return _opts.DevBypass;
    }

    // DevBypass mark (si lo usas en pruebas)
    if (user.HasClaim(c => c.Type == "dev" && string.Equals(c.Value, "true", StringComparison.OrdinalIgnoreCase)))
    {
      Console.WriteLine($"[DEBUG AUTH] DevBypass claim found, allowing access");
      return true;
    }

    var userGroups = user.GetGroupValues().ToArray();
    Console.WriteLine($"[DEBUG AUTH] User groups: [{string.Join(", ", userGroups)}]");

    // Compara grupos del usuario vs. lista permitida para el scope
    var hasAccess = user.HasAnyGroup(allowed);
    Console.WriteLine($"[DEBUG AUTH] Access granted: {hasAccess}");
    return hasAccess;
  }

  public bool IsAuthorizedForAny(ClaimsPrincipal? user, params string[] scopes)
  {
    if (scopes is null || scopes.Length == 0) return false;
    foreach (var s in scopes)
      if (IsAuthorized(user, s)) return true;
    return false;
  }
}
