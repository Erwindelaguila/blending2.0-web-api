using Function.Blending.Opt.Functions.Support.Execution;
using Function.Blending.Opt.Shared.Extensions; // GetUserId() de Claims

// Alias EF model
using Ef = Function.Blending.Opt.Infrastructure.Persistence.Models;

namespace Function.Blending.Opt.Functions.Support.Logging;

/// <summary>
/// Compone filas para dbo.SysLog a partir de excepciones o mensajes,
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
  /// Crea un SysLog con Level=error (por defecto) a partir de una excepción.
  /// </summary>
  public Ef.SysLog FromException(Exception ex, string level = "error", string? extraInfo = null)
  {
    var (userId, username) = ResolveUser();
    var (requestId, functionId) = ResolveInvocationIds();

    return new Ef.SysLog
    {
      Id = Guid.NewGuid(),
      NameSpace = _namespace,
      ClassName = _className,
      MethodName = methodName,
      Username = username,
      UserId = userId,
      Message = ex.Message,
      StackTrace = ex.ToString(),
      ExtraInfo = extraInfo,
      RequestInvocationId = requestId,
      FunctionInvocationId = functionId,
      ExceptionGroupId = null,
      Level = level
    };
  }

  /// <summary>
  /// Crea un SysLog para un mensaje arbitrario (debug/info/warning/error).
  /// </summary>
  public Ef.SysLog FromMessage(string message, string level, string? extraInfo = null)
  {
    var (userId, username) = ResolveUser();
    var (requestId, functionId) = ResolveInvocationIds();

    return new Ef.SysLog
    {
      Id = Guid.NewGuid(),
      NameSpace = _namespace,
      ClassName = _className,
      MethodName = methodName,
      Username = username,
      UserId = userId,
      Message = message,
      StackTrace = null,
      ExtraInfo = extraInfo,
      RequestInvocationId = requestId,
      FunctionInvocationId = functionId,
      ExceptionGroupId = null,
      Level = string.IsNullOrWhiteSpace(level) ? "error" : level.ToLowerInvariant()
    };
  }

  private (Guid? userId, string? username) ResolveUser()
  {
    // Username ya está expuesto por IRequestContext (Username). :contentReference[oaicite:3]{index=3}
    var username = requestContext.Username;

    // UserId lo resolvemos desde los claims usando tu extensión GetUserId()
    // (por ejemplo oid/sub) y lo parseamos a Guid?.
    Guid? userId = null;
    var uidStr = requestContext.User?.GetUserId();
    if (!string.IsNullOrWhiteSpace(uidStr) && Guid.TryParse(uidStr, out var g))
      userId = g;

    return (userId, username);
  }

  private (Guid? requestId, Guid? functionId) ResolveInvocationIds()
  {
    // FunctionInvocationId viene de FunctionContext.InvocationId (string),
    // lo parseamos si es Guid.
    Guid? functionId = null;
    var inv = functionContextAccessor.Current?.InvocationId;
    if (!string.IsNullOrWhiteSpace(inv) && Guid.TryParse(inv, out var g))
      functionId = g;

    // RequestInvocationId: usamos el CorrelationId del request si es GUID; si no, null.
    Guid? requestId = null;
    var corr = requestContext.CorrelationId; // expuesto por IRequestContext. :contentReference[oaicite:4]{index=4}
    if (!string.IsNullOrWhiteSpace(corr) && Guid.TryParse(corr, out var r))
      requestId = r;

    return (requestId, functionId);
  }
}
