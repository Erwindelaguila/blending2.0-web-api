using Function.Blending.Opt.Domain.ReadModels;

namespace Function.Blending.Opt.Domain.Abstractions.Services;

public interface ICalInpParameterService
{
  Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersByExecutionIdAsync(Guid ejecucionId, CancellationToken ct);
}
