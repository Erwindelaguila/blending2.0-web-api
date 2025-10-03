using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.Logging;

namespace Function.Blending.Opt.Infrastructure.Services.Logging;

public sealed class SysLogService(ISysLogRepository repo) : ISysLogService
{
  public Task WriteAsync(SysLogRecord entry, CancellationToken ct = default)
  {
    // Asegurar Id
    var normalized = entry with { Id = entry.Id == Guid.Empty ? Guid.NewGuid() : entry.Id };
    return repo.WriteAsync(normalized, ct);
  }
}
