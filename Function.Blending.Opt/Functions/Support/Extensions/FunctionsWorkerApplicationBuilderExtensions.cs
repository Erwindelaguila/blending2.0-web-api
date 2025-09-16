using Function.Blending.Opt.Functions.Pipeline;
using Function.Blending.Opt.Shared.Constants;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using System.Linq;

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
    builder.UseMiddleware<FunctionContextAccessorMiddleware>();

    // 2) CorrelationId: escribe Items["CorrelationId"] y header de respuesta.
    builder.UseMiddleware<CorrelationIdMiddleware>();

    // 3) Middlewares que se prenden/apagan por bandera (gating en composición).
    builder.UseIf<ExceptionHandlingMiddleware>(enableExceptionHandling);
    builder.UseIf<RequestLoggingMiddleware>(enableRequestLogging);

    // 4) Middlewares que aplican SOLO a HTTP triggers (gating por contexto).
    if (enableRequestSizeLimit)
    {
      builder.UseWhen<RequestSizeLimitMiddleware>(ctx => ctx.FunctionDefinition.InputBindings.Values.Any(b => b.Type == MiscellaneousConstants.HttpTrigger));
    }

    // HMAC solo si es HTTP trigger
    builder.UseWhen<HmacValidationMiddleware>(ctx => ctx.FunctionDefinition.InputBindings.Values.Any(b => b.Type == MiscellaneousConstants.HttpTrigger));

    // 5) Autenticación/Autorización (si está habilitada)
    if (enableAuthentication)
    {
      builder.UseMiddleware<PrincipalResolutionMiddleware>();
      builder.UseMiddleware<AuthorizationMiddleware>();
    }

    return builder;
  }
}
