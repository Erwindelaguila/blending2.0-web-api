using Function.Blending.Opt.Domain.ReadModels;

namespace Function.Blending.Opt.Domain.Abstractions.Repositories;

public interface ICalInpParameterRepository
{
  Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersByEjecucionIdAsync(Guid ejecucionId, CancellationToken ct);
}
