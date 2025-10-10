using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Functions.Support.Logging; // SysLogComposer
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Function.Blending.Opt.Functions.Support.ProblemDetails;

public static class ProblemDetailsLoggingExtensions
{
  /// <summary>
  /// Registra la excepción en SysLog (Dominio) y devuelve un Problem Details con el status indicado.
  /// </summary>
  public static async Task<HttpResponseData> CreateWithLogAsync(
    this IProblemDetailsWriter problem,
    FunctionContext fctx,
    HttpRequestData req,
    ISysLogService syslog,
    IRequestContext requestContext,
    IFunctionContextAccessor fctxAccessor,
    HttpStatusCode status,
    string type,
    string title,
    string detail,
    Exception ex,
    string? extraInfo = null,
    IDictionary<string, object?>? extensions = null,
    Type? sourceType = null,
    string? methodName = null)
  {
    // 1) Registrar en SysLog
    var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType ?? typeof(ProblemDetailsLoggingExtensions), methodName);

    var record = composer.FromException(ex, SysLogLevel.Error, extraInfo);
    await syslog.WriteAsync(record, fctx.CancellationToken);

    // 2) Responder con Problem Details
    return await problem.CreateAsync(fctx, req, status, type, title, detail, extensions);
  }

  /// <summary>
  /// Azúcar para 400 BadRequest con log.
  /// </summary>
  public static Task<HttpResponseData> BadRequestWithLogAsync(
    this IProblemDetailsWriter problem,
    FunctionContext fctx,
    HttpRequestData req,
    ISysLogService syslog,
    IRequestContext requestContext,
    IFunctionContextAccessor fctxAccessor,
    string type,
    string title,
    string detail,
    Exception ex,
    string? extraInfo = null,
    IDictionary<string, object?>? extensions = null,
    Type? sourceType = null,
    string? methodName = null)
    => problem.CreateWithLogAsync(fctx, req, syslog, requestContext, fctxAccessor, HttpStatusCode.BadRequest, type, title, detail, ex, extraInfo, extensions, sourceType, methodName);

  public static async Task<HttpResponseData> CreateBusinessErrorAsync(
  this IProblemDetailsWriter problem,
  FunctionContext fctx,
  HttpRequestData req,
  ISysLogService syslog,
  IRequestContext requestContext,
  IFunctionContextAccessor fctxAccessor,
  HttpStatusCode status,
  string type, string title, string detail,
  string? extraInfo = null,
  IDictionary<string, object?>? extensions = null,
  Type? sourceType = null,
  string? methodName = null)
  {
    var composer = new SysLogComposer(requestContext, fctxAccessor, sourceType ?? typeof(ProblemDetailsLoggingExtensions), methodName);
    var record = composer.FromMessage(detail, SysLogLevel.Warning, extraInfo);
    await syslog.WriteAsync(record, fctx.CancellationToken);

    return await problem.CreateAsync(fctx, req, status, type, title, detail, extensions);
  }
}
