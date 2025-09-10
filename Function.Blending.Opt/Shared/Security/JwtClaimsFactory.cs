using Function.Blending.Opt.Shared.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Function.Blending.Opt.Shared.Security;

/// <summary>
/// Fábrica reutilizable para construir Claims/Identity a partir de un JwtSecurityToken,
/// independiente del modo de validación (Relaxed/Strict).
/// </summary>
public static class JwtClaimsFactory
{
  public sealed class Options
  {
    /// <summary>AuthType a usar en la ClaimsIdentity resultante.</summary>
    public string AuthType { get; init; } = MiscellaneousKeys.Bearer;

    /// <summary>Si no es null/empty, agrega un claim ("auth_mode", value).</summary>
    public string? AuthModeTag { get; init; }

    /// <summary>Valida expiración con jwt.ValidTo contra DateTime.UtcNow (útil en Relaxed).</summary>
    public bool ValidateLifetime { get; init; } = false;

    /// <summary>Incluye claims de “groups”.</summary>
    public bool IncludeGroups { get; init; } = true;

    /// <summary>Incluye claims de “wids” (roles de directorio).</summary>
    public bool IncludeWids { get; init; } = true;

    /// <summary>Incluye claims de “roles”.</summary>
    public bool IncludeRoles { get; init; } = true;

    /// <summary>Incluye claim “scope” (a partir de “scp” o “scope”).</summary>
    public bool IncludeScope { get; init; } = true;
  }

  /// <summary>
  /// Construye un ClaimsPrincipal a partir de un JwtSecurityToken, aplicando opciones.
  /// No valida firma; asume que el JWT ya fue validado aguas arriba (modo Strict), o que
  /// estás en un modo Relaxed donde sólo se quiere mapear claims.
  /// </summary>
  public static ClaimsPrincipal? CreatePrincipal(JwtSecurityToken jwt, Options? opts = null)
  {
    var identity = CreateIdentity(jwt, opts);
    return identity is null ? null : new ClaimsPrincipal(identity);
  }

  /// <summary>
  /// Construye una ClaimsIdentity a partir de un JwtSecurityToken, aplicando opciones.
  /// </summary>
  public static ClaimsIdentity? CreateIdentity(JwtSecurityToken jwt, Options? opts = null)
  {
    opts ??= new Options();

    if (opts.ValidateLifetime && jwt.ValidTo < DateTime.UtcNow)
      return null;

    var claims = BuildClaims(jwt, opts);
    return new ClaimsIdentity(claims, opts.AuthType);
  }

  /// <summary>
  /// Convierte JwtSecurityToken → IEnumerable&lt;Claim&gt; según las opciones.
  /// </summary>
  public static IEnumerable<Claim> BuildClaims(JwtSecurityToken jwt, Options opts)
  {
    var claims = new List<Claim>();

    void addIf(string type, string? val)
    {
      if (!string.IsNullOrWhiteSpace(val))
        claims.Add(new Claim(type, val));
    }

    string? First(params string[] keys)
      => jwt.Claims.FirstOrDefault(c => keys.Contains(c.Type, StringComparer.OrdinalIgnoreCase))?.Value;

    // Identificadores
    addIf(ClaimTypesKeys.Oid, First(ClaimTypesKeys.Oid));
    addIf(ClaimTypesKeys.Sub, First(ClaimTypesKeys.Sub));

    // Nombre/usuario
    addIf(ClaimTypesKeys.Name, First(ClaimTypesKeys.Name, ClaimTypesKeys.UniqueName));
    addIf(ClaimTypesKeys.PreferredUsername, First(ClaimTypesKeys.PreferredUsername));
    addIf(ClaimTypesKeys.GivenName, First(ClaimTypesKeys.GivenName));
    addIf(ClaimTypesKeys.FamilyName, First(ClaimTypesKeys.FamilyName));
    addIf(ClaimTypesKeys.Upn, First(ClaimTypesKeys.Upn));

    // Cliente/app de origen
    addIf(ClaimTypesKeys.Azp, First(ClaimTypesKeys.Azp));
    addIf(ClaimTypesKeys.AppId, First(ClaimTypesKeys.AppId));
    addIf(ClaimTypesKeys.ClientId, First(ClaimTypesKeys.ClientId));

    // Scopes
    if (opts.IncludeScope)
    {
      var scope = First(ClaimTypesKeys.Scp, ClaimTypesKeys.Scope);
      addIf(ClaimTypesKeys.Scope, scope);
    }

    // Groups
    if (opts.IncludeGroups)
    {
      foreach (var g in jwt.Claims.Where(c => c.Type.Equals(ClaimTypesKeys.Groups, StringComparison.OrdinalIgnoreCase)).Select(c => c.Value))
        claims.Add(new Claim(ClaimTypesKeys.Groups, g));
    }

    // WIDS (roles de directorio)
    if (opts.IncludeWids)
    {
      foreach (var r in jwt.Claims.Where(c => c.Type.Equals(ClaimTypesKeys.Wids, StringComparison.OrdinalIgnoreCase)).Select(c => c.Value))
        claims.Add(new Claim(ClaimTypesKeys.Wids, r));
    }

    // Roles
    if (opts.IncludeRoles)
    {
      foreach (var r in jwt.Claims.Where(c => c.Type.Equals(ClaimTypesKeys.Roles, StringComparison.OrdinalIgnoreCase)).Select(c => c.Value))
        claims.Add(new Claim(ClaimTypes.Role, r));
    }

    if (!string.IsNullOrWhiteSpace(opts.AuthModeTag))
      claims.Add(new Claim(ClaimTypesKeys.AuthMode, opts.AuthModeTag!));

    return claims;
  }
}
