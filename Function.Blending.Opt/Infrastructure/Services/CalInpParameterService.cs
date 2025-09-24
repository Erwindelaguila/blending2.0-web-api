using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ReadModels;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class CalInpParameterService(ICalInpParameterRepository repo): ICalInpParameterService
{
  public async Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersByExecutionIdAsync(Guid ejecucionId, CancellationToken ct) => await repo.GetParametersByEjecucionIdAsync(ejecucionId, ct);
}
