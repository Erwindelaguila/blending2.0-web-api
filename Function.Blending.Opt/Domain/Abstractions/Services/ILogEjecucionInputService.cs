using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ILogEjecucionInputService
{
  Task<LogInpInfo> GetInfoByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<LogInpOferta> GetOfertaByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<LogInpFiltro> GetFiltroByExecutionIdAsync(Guid executionId, CancellationToken ct);
}
