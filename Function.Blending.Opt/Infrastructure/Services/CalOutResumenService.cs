using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class CalOutResumenService(ICalEjecucionOutputRepository repo) : ICalEjecucionOutService
{
  public Task<IReadOnlyList<CalOutResumen>> GetSummariesByExcecutionAsync(Guid executionId, CancellationToken ct) => repo.GetSummariesByExcecutionAsync(executionId, ct);

  public async Task<IReadOnlyList<CalOutResumen>> GetDeepSummariesByExcecutionAsync(Guid executionId, CancellationToken ct) => await repo.GetDeepSummariesByExcecutionAsync(executionId, ct);

  public async Task<IReadOnlyList<CalOutDetalle>> GetDetailsByExcecutionAsync(Guid executionId, CancellationToken ct) => await repo.GetDetailsByExcecutionAsync(executionId, ct);

  public async Task<IReadOnlyList<CalOutDetalle>> GetDeepDetailsByExcecutionAsync(Guid executionId, CancellationToken ct) => await repo.GetDeepDetailsByExcecutionAsync(executionId, ct);
}
