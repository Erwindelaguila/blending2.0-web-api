using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class CalOutResumenService(ICalOutResumenRepository repo) : ICalOutResumenService
{
  public Task<IReadOnlyList<CalOutResumen>> GetAllByExcecutionAsync(Guid ejecucionId, CancellationToken ct) => repo.GetAllByExcecutionAsync(ejecucionId, ct);
}
