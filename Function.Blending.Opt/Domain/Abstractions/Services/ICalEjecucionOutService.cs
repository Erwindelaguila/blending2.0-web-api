using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ICalEjecucionOutService
{
  Task<IReadOnlyList<CalOutResumen>> GetSummariesByExcecutionAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<CalOutResumen>> GetDeepSummariesByExcecutionAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<CalOutDetalle>> GetDetailsByExcecutionAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<CalOutDetalle>> GetDeepDetailsByExcecutionAsync(Guid executionId, CancellationToken ct);
}