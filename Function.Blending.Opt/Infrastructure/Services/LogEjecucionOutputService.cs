using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class LogEjecucionOutputService(ILogEjecucionOutputRepository repo) : ILogEjecucionOutputService
{
  public Task<IReadOnlyList<LogOutContenedor>> GetContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct) => repo.GetContenedoresByExcecutionAsync(executionId, ct);

  public Task<IReadOnlyList<LogOutContenedor>> GetDeepContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct) => repo.GetDeepContenedoresByExcecutionAsync(executionId, ct);
}
