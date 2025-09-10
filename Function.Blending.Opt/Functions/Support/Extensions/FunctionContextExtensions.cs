using System.Security.Claims;
using Function.Blending.Opt.Shared.Constants;
using Function.Blending.Opt.Shared.Extensions;
using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Opt.Functions.Support.Extensions;

public static class FunctionContextExtensions
{
  /// <summary>
  /// Devuelve el CorrelationId almacenado en el contexto (si existe).
  /// </summary>
  public static string? GetCorrelationId(this FunctionContext ctx)
    => ctx.Items.TryGetValue("CorrelationId", out var v) ? v?.ToString() : null;

  /// <summary>
  /// Devuelve el ClaimsPrincipal resuelto por middlewares previos (si existe).
  /// </summary>
  public static ClaimsPrincipal? GetUser(this FunctionContext ctx)
    => ctx.Items.TryGetValue("Principal", out var v) ? v as ClaimsPrincipal : null;

  /// <summary>
  /// Devuelve los IDs de grupos/roles del usuario, normalizados y sin duplicados.
  /// (usa ClaimTypesEx.GroupClaimTypes)
  /// </summary>
  public static IEnumerable<string> GetUserGroupIds(this FunctionContext ctx)
    => ctx.GetUser()?.GetGroupValues() ?? [];

  // ===== Helpers adicionales (no rompen API existente) =====

  /// <summary>
  /// Devuelve el ObjectId (oid) del usuario o "sub" como fallback (si existe).
  /// </summary>
  public static string? GetUserObjectId(this FunctionContext ctx)
    => ctx.GetUser()?.GetUserId();

  /// <summary>
  /// Devuelve el username (preferred_username/upn/unique_name/Name) del usuario (si existe).
  /// </summary>
  public static string? GetUsername(this FunctionContext ctx)
    => ctx.GetUser()?.GetUsername();
}
