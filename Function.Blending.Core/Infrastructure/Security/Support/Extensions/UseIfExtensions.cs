using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;

namespace Function.Blending.Core.Infrastructure.Security.Support.Extensions;

/// <summary>
/// Azúcar sintáctico para habilitar/deshabilitar middlewares en tiempo de composición.
/// </summary>
public static class UseIfExtensions
{
  /// <summary>
  /// Registra el middleware solo si <paramref name="enabled"/> es true.
  /// </summary>
  public static IFunctionsWorkerApplicationBuilder UseIf<TMiddleware>(
      this IFunctionsWorkerApplicationBuilder builder,
      bool enabled)
      where TMiddleware : class, IFunctionsWorkerMiddleware
      => enabled ? builder.UseMiddleware<TMiddleware>() : builder;

  /// <summary>
  /// Registra el middleware con predicado por contexto, solo si <paramref name="enabled"/> es true.
  /// Útil cuando quieres combinar una bandera (feature toggle) con una condición por Function/trigger.
  /// </summary>
  public static IFunctionsWorkerApplicationBuilder UseIfWhen<TMiddleware>(
      this IFunctionsWorkerApplicationBuilder builder,
      bool enabled,
      Func<FunctionContext, bool> predicate)
      where TMiddleware : class, IFunctionsWorkerMiddleware
      => enabled ? builder.UseWhen<TMiddleware>(predicate) : builder;
}
