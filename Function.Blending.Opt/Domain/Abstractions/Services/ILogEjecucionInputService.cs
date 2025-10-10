using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ILogEjecucionInputService
{
  Task<LogInpInfo> GetInfoByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<LogInpFiltro> GetFiltroByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<LogInpDemanda> GetDemandaByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<LogInpOferta>> GetOfertaByExecutionIdAsync(Guid executionId, CancellationToken ct);
}
