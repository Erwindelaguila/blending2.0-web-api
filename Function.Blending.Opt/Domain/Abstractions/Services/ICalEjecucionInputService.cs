using Function.Blending.Opt.Domain.ValueObjects;
using Function.Blending.Opt.Domain.ReadModels;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ICalEjecucionInputService
{
  Task<CalInpFiltro> GetFilterByExecutionIdAsync(Guid executionId, CancellationToken ct);
  Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersItemRmByExecutionIdAsync(Guid executionId, CancellationToken ct);
}
