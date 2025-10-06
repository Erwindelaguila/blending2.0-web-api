using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ICalEjecucionInputRepository
{
  Task<CalInpFiltro> GetFilterByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersItemRmByExecutionIdAsync(Guid executionId, CancellationToken ct);
}
