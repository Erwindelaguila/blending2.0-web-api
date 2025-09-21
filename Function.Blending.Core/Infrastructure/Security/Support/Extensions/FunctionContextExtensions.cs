using System.Security.Claims;
using Function.Blending.Core.Infrastructure.Security.Shared.Constants;
using Function.Blending.Core.Infrastructure.Security.Shared.Extensions;
using Function.Blending.Core.Infrastructure.Security.Support.Http;
using Microsoft.Azure.Functions.Worker;

namespace Function.Blending.Core.Infrastructure.Security.Support.Extensions;

public static partial class FunctionContextExtensions
{
  /// <summary>
  /// Devuelve el CorrelationId almacenado en el contexto (si existe).
  /// </summary>
  public static string? GetCorrelationId(this FunctionContext ctx) => ctx.Items.TryGetValue(CorrelationKeys.CorrelationIdItemKey, out var v) ? v?.ToString() : null;

  /// <summary>
  /// Devuelve el ClaimsPrincipal resuelto por middlewares previos (si existe).
  /// </summary>
  public static ClaimsPrincipal? GetUser(this FunctionContext ctx) => ctx.Items.TryGetValue(MiscellaneousConstants.Principal, out var v) ? v as ClaimsPrincipal : null;

  /// <summary>
  /// Devuelve los IDs de grupos/roles del usuario, normalizados y sin duplicados.
  /// (usa ClaimTypesEx.GroupClaimTypes)
  /// </summary>
  public static IEnumerable<string> GetUserGroupIds(this FunctionContext ctx) => ctx.GetUser()?.GetGroupValues() ?? [];

  /// <summary>
  /// Devuelve el username (preferred_username/upn/unique_name/Name) del usuario (si existe).
  /// </summary>
  public static string? GetUsername(this FunctionContext ctx) => ctx.GetUser()?.GetUsername();
}
