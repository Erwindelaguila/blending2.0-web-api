using Function.Blending.Opt.Domain.Logging;
using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Shared.Extensions; // GetUserId()

namespace Function.Blending.Opt.Functions.Support.Logging;

/// <summary>
/// Compone un Domain.SysLogRecord (neutral a EF) a partir de excepciones o mensajes,
/// usando únicamente lo que expone IRequestContext y el FunctionContext actual.
/// </summary>
public sealed class SysLogComposer(
  IRequestContext requestContext,
  IFunctionContextAccessor functionContextAccessor,
  Type sourceType,
  string? methodName = null)
{
  private readonly string _namespace = sourceType.Namespace ?? string.Empty;
  private readonly string? _className = sourceType.Name;

  /// <summary>
  /// Genera un SysLogRecord para una excepción.
  /// </summary>
  public SysLogRecord FromException(Exception ex, SysLogLevel level = SysLogLevel.Error, string? extraInfo = null)
  {
    var (userId, username) = ResolveUser();
    var (requestId, traceparent, functionId) = ResolveInvocationIds();

    return new SysLogRecord(
      Id: Guid.NewGuid(),
      NameSpace: _namespace,
      ClassName: _className,
      MethodName: methodName,
      Username: username,
      UserId: userId,
      Message: ex.Message,
      StackTrace: ex.ToString(),
      ExtraInfo: extraInfo,
      TraceParentId: traceparent,
      RequestInvocationId: requestId,
      FunctionInvocationId: functionId,
      ExceptionGroupId: null,
      Level: level
    );
  }

  /// <summary>
  /// Genera un SysLogRecord para un mensaje arbitrario (debug|info|warning|error).
  /// </summary>
  public SysLogRecord FromMessage(string message, SysLogLevel level, string? extraInfo = null)
  {
    var (userId, username) = ResolveUser();
    var (requestId, traceparent, functionId) = ResolveInvocationIds();

    return new SysLogRecord(
      Id: Guid.NewGuid(),
      NameSpace: _namespace,
      ClassName: _className,
      MethodName: methodName,
      Username: username,
      UserId: userId,
      Message: message,
      StackTrace: null,
      ExtraInfo: extraInfo,
      TraceParentId: traceparent,
      RequestInvocationId: requestId,
      FunctionInvocationId: functionId,
      ExceptionGroupId: null,
      Level: level
    );
  }

  private (Guid? userId, string? username) ResolveUser()
  {
    // Username desde IRequestContext
    var username = requestContext.Username;

    // UserId desde claims (oid/sub) usando tu extensión GetUserId()
    Guid? userId = null;
    var uidStr = requestContext.User?.GetUserId();
    if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var g))
      userId = g;

    return (userId, username);
  }

  private (Guid? requestId, string? traceparent, Guid? functionId) ResolveInvocationIds()
  {
    // FunctionInvocationId desde FunctionContext.InvocationId (si es GUID)
    Guid? functionId = null;
    var inv = functionContextAccessor.Current?.InvocationId;
    if (!string.IsNullOrWhiteSpace(inv) && Guid.TryParse(inv, out var g))
      functionId = g;

    // RequestInvocationId: usa CorrelationId si es GUID
    Guid? requestId = null;
    var corr = requestContext.CorrelationId;
    if (!string.IsNullOrWhiteSpace(corr) && Guid.TryParse(corr, out var r))
      requestId = r;

    var traceparent = requestContext.TraceParent;

    return (requestId, traceparent, functionId);
  }
}
