using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Http;
using Function.Blending.Opt.Functions.Support.Logging;   // SysLogComposer
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Runtime.CompilerServices;

namespace Function.Blending.Opt.Functions.Support.Extensions;

public static class HttpRequestDataJsonLoggingExtensions
{
  /// <summary>
  /// Deserializa JSON y, si falla, registra el error con SysLogComposer (Domain) y devuelve default.
  /// Usa Caller Info para inferir className/methodName sin repetición.
  /// </summary>
  public static Task<T?> TryReadJsonWithSysLogAsync<T>(
      this HttpRequestData req,
      FunctionContext fctx,
      IRequestContext requestContext,
      IFunctionContextAccessor fctxAccessor,
      ISysLogService syslog,
      SysLogLevel level = SysLogLevel.Error,
      [CallerMemberName] string? caller = null,
      [CallerFilePath] string? file = null)
  {
    var sourceType = ResolveSourceType(file) ?? typeof(HttpRequestDataJsonLoggingExtensions);

    return req.TryReadFromJsonOrRawAsync<T>(fctx, async (ex, raw) =>
    {
      var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType, caller);
      var record = composer.FromException(ex, level, raw);
      await syslog.WriteAsync(record, fctx.CancellationToken); // usa el token del FunctionContext
    }, fctx.CancellationToken);

    //return req.TryReadJsonAsync<T>(async (ex, raw) =>
    //{
    //  var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType, caller);
    //  var record = composer.FromException(ex, level, raw);
    //  await syslog.WriteAsync(record, fctx.CancellationToken); // usa el token del FunctionContext
    //});
  }

  private static Type? ResolveSourceType(string? callerFilePath)
  {
    // Si quieres mapear el archivo .cs a un Type real lo implementas aquí.
    // Por ahora, mantenemos null → usa el helper como source.
    return null;
  }
}
