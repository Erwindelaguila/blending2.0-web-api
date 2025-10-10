using System;
using System.Threading;
using System.Threading.Tasks;
using Function.Blending.Opt.Domain.Abstractions.Repositories;
using Function.Blending.Opt.Domain.Abstractions.Services;

namespace Function.Blending.Opt.Infrastructure.Services;

/// <summary>Capa de servicio minimal: puede crecer (métricas, validaciones, tracing) sin tocar EF.</summary>
public sealed class SysParamService(ISysParamRepository repo) : ISysParamService
{
  public Task<Guid?> GetIdAsync(string sysParamKey, CancellationToken ct) => repo.GetIdAsync(sysParamKey, ct);

  public async Task<Guid> GetRequiredIdAsync(string sysParamKey, CancellationToken ct)
  {
    var id = await repo.GetIdAsync(sysParamKey, ct);
    return id is null ? throw new InvalidOperationException($"SysParam key '{sysParamKey}' not found or inactive.") : id.Value;
  }
}
