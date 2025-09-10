using Function.Blending.Opt.Functions.Pipeline;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

namespace Function.Blending.Opt.Functions.Support.Extensions;

public static class FunctionsWorkerApplicationBuilderExtensions
{
  /// <summary>
  /// Pipeline único y ordenado para TODAS las Functions HTTP.
  /// </summary>
  public static IFunctionsWorkerApplicationBuilder UseFunctionsPipeline(
      this IFunctionsWorkerApplicationBuilder builder,
      bool enableExceptionHandling = true,
      bool enableRequestLogging = true,
      bool enableRequestSizeLimit = true,
      bool enableAuthentication = true)
  {
    // IMPORTANTÍSIMO: el accessor debe ir PRIMERO para que IRequestContext pueda leer el FunctionContext actual.
    builder.UseWhen<FunctionContextAccessorMiddleware>(_ => true); // <-- clave

    // CorrelationId: genera/inyecta el ID en Items["CorrelationId"] y añade header a la respuesta.
    builder.UseWhen<CorrelationIdMiddleware>(_ => true);

    if (enableExceptionHandling)
      builder.UseWhen<ExceptionHandlingMiddleware>(_ => true);

    if (enableRequestLogging)
      builder.UseWhen<RequestLoggingMiddleware>(_ => true);

    if (enableRequestSizeLimit)
      builder.UseWhen<RequestSizeLimitMiddleware>(ctx =>
          ctx.FunctionDefinition.InputBindings.Values.Any(b => b.Type == "httpTrigger"));

    // HMAC se activa automáticamente si la Function tiene el atributo correspondiente
    builder.UseWhen<HmacValidationMiddleware>(ctx =>
        ctx.FunctionDefinition.InputBindings.Values.Any(b => b.Type == "httpTrigger"));

    if (enableAuthentication)
    {
      builder.UseWhen<PrincipalResolutionMiddleware>(_ => true);
      builder.UseWhen<AuthorizationMiddleware>(_ => true);
    }

    return builder;
  }
}
