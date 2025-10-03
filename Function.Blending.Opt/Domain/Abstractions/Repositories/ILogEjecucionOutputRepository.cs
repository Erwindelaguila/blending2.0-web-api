using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ILogEjecucionOutputRepository
{
  Task<IReadOnlyList<LogOutContenedor>> GetContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<LogOutContenedor>> GetDeepContenedoresByExcecutionAsync(Guid executionId, CancellationToken ct);
}
