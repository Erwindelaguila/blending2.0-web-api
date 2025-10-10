using Function.Blending.Opt.Domain.Logging;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

/// <summary>
/// Orquestación simple sobre el repo (completa defaults, valida, etc.)
/// </summary>
public interface ISysLogService
{
  Task WriteAsync(SysLogRecord entry, CancellationToken ct = default);
}
