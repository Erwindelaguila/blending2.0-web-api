using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class LogEjecucionInputService(ILogEjecucionInputRepository repo) : ILogEjecucionInputService
{
  public Task<LogInpFiltro> GetFiltroByExecutionIdAsync(Guid executionId, CancellationToken ct) => repo.GetFiltroByExecutionIdAsync(executionId, ct);

  public Task<LogInpInfo> GetInfoByExecutionIdAsync(Guid executionId, CancellationToken ct) => repo.GetInfoByExecutionIdAsync(executionId, ct);

  public Task<LogInpOferta> GetOfertaByExecutionIdAsync(Guid executionId, CancellationToken ct) => repo.GetOfertaByExecutionIdAsync(executionId, ct);
}
