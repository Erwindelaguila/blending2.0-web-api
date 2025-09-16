using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ICalOutResumenService
{
  Task<IReadOnlyList<CalOutResumen>> GetAllByExcecutionAsync(Guid ejecucionId, CancellationToken ct);
}