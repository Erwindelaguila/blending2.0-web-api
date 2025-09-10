using System.Security.Claims;

namespace Function.Blending.Opt.Shared.Constants;

/// <summary>
/// Tipos de claims centralizados (evita strings mágicos y listas duplicadas).
/// </summary>
public static class ClaimTypesEx
{
  /// <summary>Claims que representan grupos/roles del usuario.</summary>
  public static readonly string[] GroupClaimTypes =
  [
    ClaimTypesKeys.Groups,
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/groups",
    ClaimTypesKeys.Roles,
    ClaimTypes.Role
  ];

  /// <summary>Identificadores del usuario (preferencia: oid, fallback: sub).</summary>
  public static readonly string[] UserIdClaimTypes =
  [
    ClaimTypesKeys.Oid,
    ClaimTypesKeys.Sub
  ];

  /// <summary>Usuario (login/email).</summary>
  public static readonly string[] UsernameClaimTypes =
  [
    ClaimTypesKeys.PreferredUsername,
    ClaimTypesKeys.Upn,
    ClaimTypesKeys.UniqueName,
    ClaimTypes.Name
  ];

  /// <summary>Nombres.</summary>
  public static readonly string[] GivenNameClaimTypes =
  [
    ClaimTypesKeys.GivenName,
    ClaimTypes.GivenName
  ];

  /// <summary>Apellidos.</summary>
  public static readonly string[] FamilyNameClaimTypes =
  [
    ClaimTypesKeys.FamilyName,
    ClaimTypes.Surname
  ];

  /// <summary>Scopes en access tokens.</summary>
  public static readonly string[] ScopeClaimTypes =
  [
    ClaimTypesKeys.Scope,
    ClaimTypesKeys.Scp
  ];

  /// <summary>Cliente/app de origen (SPA o app).</summary>
  public static readonly string[] ClientIdClaimTypes =
  [
    ClaimTypesKeys.Azp,
    ClaimTypesKeys.AppId,
    ClaimTypesKeys.ClientId
  ];
}
