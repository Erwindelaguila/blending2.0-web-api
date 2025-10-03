using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;
using Function.Blending.Opt.Domain.ReadModels;
using Function.Blending.Opt.Domain.ValueObjects;

namespace Function.Blending.Opt.Infrastructure.Services;

public sealed class CalInpParameterService(ICalEjecucionInputRepository repo): ICalEjecucionInputService
{
  public async Task<CalInpFiltro> GetFilterByExecutionIdAsync(Guid executionId, CancellationToken ct) => await repo.GetFilterByExecutionIdAsync(executionId, ct);

  public async Task<IReadOnlyList<CalInpParametroItemRm>> GetParametersItemRmByExecutionIdAsync(Guid executionId, CancellationToken ct) => await repo.GetParametersItemRmByExecutionIdAsync(executionId, ct);
}
