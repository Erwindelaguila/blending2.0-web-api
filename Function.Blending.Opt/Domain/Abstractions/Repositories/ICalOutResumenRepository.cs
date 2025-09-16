using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ICalOutResumenRepository
{
  Task<IReadOnlyList<CalOutResumen>> GetAllByExcecutionAsync(Guid ejecucionId, CancellationToken ct);
}
