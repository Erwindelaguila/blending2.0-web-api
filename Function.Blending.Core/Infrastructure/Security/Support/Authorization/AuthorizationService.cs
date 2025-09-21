using System.Security.Claims;
using Function.Blending.Core.Infrastructure.Security.Shared.Extensions;
using Function.Blending.Upload.Functions.Configuration.Options;
using Microsoft.Extensions.Options;

namespace Function.Blending.Core.Infrastructure.Security.Support.Authorization;

public sealed class AuthorizationService : IAuthorizationService
{
  private readonly AuthorizationOptions _opts;
  public AuthorizationService(IOptions<AuthorizationOptions> opts) => _opts = opts.Value;

  public bool IsAuthorized(ClaimsPrincipal? user, string scope)
  {
    if (!_opts.Allow.TryGetValue(scope, out var allowed) || allowed is null || allowed.Length == 0)
      return false;

    // Si no hay principal, solo pasa si DevBypass est� activo
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
