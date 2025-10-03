using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Extensions; // TryReadJsonAsync
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Runtime.CompilerServices;

namespace Function.Blending.Opt.Functions.Support.Logging;

public static class JsonRequestReader
{
  /// <summary>
  /// Igual que la extensión (HttpRequestDataJsonLoggingExtensions), pero como helper explícito (útil en pruebas o si no quieres métodos de extensión).
  /// </summary>
  public static Task<T?> TryReadWithSysLogAsync<T>(
      HttpRequestData req,
      FunctionContext fctx,
      IRequestContext requestContext,
      IFunctionContextAccessor fctxAccessor,
      ISysLogService syslog,
      SysLogLevel level = SysLogLevel.Error,
      [CallerMemberName] string? caller = null,
      [CallerFilePath] string? file = null)
  {
    var sourceType = typeof(JsonRequestReader); // o intenta resolver por file
    return req.TryReadJsonAsync<T>(async (ex, raw) =>
    {
      var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType, caller);
      var record = composer.FromException(ex, level, raw);
      await syslog.WriteAsync(record, CancellationToken.None);
    });
  }
}
