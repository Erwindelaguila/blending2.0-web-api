using Function.Blending.Opt.Functions.Configuration.Options;
using Function.Blending.Opt.Shared.Extensions;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Function.Blending.Opt.Functions.Support.Authorization;

public sealed class AuthorizationService(IOptions<AuthorizationOptions> opts) : IAuthorizationService
{
  private readonly AuthorizationOptions _opts = opts.Value;

  public bool IsAuthorized(ClaimsPrincipal? user, string scope)
  {
    if (!_opts.Allow.TryGetValue(scope, out var allowed) || allowed is null || allowed.Length == 0)
      return false;

    // Si no hay principal, solo pasa si DevBypass está activo
    if (user is null || !(user.Identity?.IsAuthenticated ?? false))
      return _opts.DevBypass;

    // DevBypass mark (si lo usas en pruebas)
    if (user.HasClaim(c => c.Type == "dev" && string.Equals(c.Value, "true", StringComparison.OrdinalIgnoreCase)))
      return true;

    // Compara grupos del usuario vs. lista permitida para el scope
    return user.HasAnyGroup(allowed);
  }

  public bool IsAuthorizedForAny(ClaimsPrincipal? user, params string[] scopes)
  {
    if (scopes is null || scopes.Length == 0) return false;
    foreach (var s in scopes)
      if (IsAuthorized(user, s)) return true;
    return false;
  }
}
