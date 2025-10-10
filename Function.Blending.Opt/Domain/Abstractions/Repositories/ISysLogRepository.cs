using Function.Blending.Opt.Domain.Logging;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ISysLogRepository
{
  Task WriteAsync(SysLogRecord entry, CancellationToken ct);
}
