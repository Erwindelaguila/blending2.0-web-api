using Function.Blending.Opt.Functions.Pipeline;
using Microsoft.Azure.Functions.Worker;
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
    // 1) ¡Clave! Primero: accessor → así IRequestContext puede leer el FunctionContext actual.
    builder.UseWhen<FunctionContextAccessorMiddleware>(_ => true);

    // 2) CorrelationId: escribe Items["CorrelationId"] y header de respuesta.
    builder.UseWhen<CorrelationIdMiddleware>(_ => true);

    if (enableExceptionHandling)
      builder.UseWhen<ExceptionHandlingMiddleware>(_ => true);

    if (enableRequestLogging)
      builder.UseWhen<RequestLoggingMiddleware>(_ => true);

    if (enableRequestSizeLimit)
      builder.UseWhen<RequestSizeLimitMiddleware>(ctx =>
          ctx.FunctionDefinition.InputBindings.Values.Any(b => b.Type == "httpTrigger"));

    // HMAC solo si es HTTP trigger
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
