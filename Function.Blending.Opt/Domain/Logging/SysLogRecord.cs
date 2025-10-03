namespace Function.Blending.Opt.Domain.Logging;

/// <summary>
/// Payload “plano” de un registro a persistir en dbo.SysLog.
/// (1:1 con columnas; el repositorio se encarga de EF)
/// </summary>
public sealed record SysLogRecord(
  Guid Id,
  string NameSpace,
  string? ClassName,
  string? MethodName,
  string? Username,
  Guid? UserId,
  string? Message,
  string? StackTrace,
  string? ExtraInfo,
  string? TraceParentId,
  Guid? RequestInvocationId,
  Guid? FunctionInvocationId,
  Guid? ExceptionGroupId,
  SysLogLevel Level
);
